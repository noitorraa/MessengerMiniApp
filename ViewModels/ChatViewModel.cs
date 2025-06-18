using MessengerMiniApp.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows.Input;

namespace MessengerMiniApp.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private const string BaseApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/";
        private const string HubUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/chatHub";
        private const long MaxFileSize = 16 * 1024 * 1024; // 16 MB
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri(BaseApiUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
        private readonly string _cacheDir = Path.Combine(FileSystem.CacheDirectory, "MessengerFiles");
        private readonly int _userId;
        private readonly int _chatId;
        private readonly Dictionary<int, MessageDto> _messagesById = new();

        private HubConnection _hubConnection;
        private string _newMessageText;
        private bool _isLoadingMore;
        private bool _hasMoreMessages = true;
        private bool _isBusy;
        private string _peerUsername;
        private string _peerAvatar;
        private string _peerStatus = "Не в сети";

        private DateTime? _lastCacheUpdate;
        private DateTime? _firstCachedMessageDate;
        public MessageDto _firstVisibleMessageBeforeLoad;

        public ObservableCollection<MessageDto> Messages { get; } = new();
        public ICommand OpenFileCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand LoadMoreCommand { get; }
        public ICommand AttachFileCommand { get; }
        public int CurrentUserId => _userId;

        public string NewMessageText
        {
            get => _newMessageText;
            set => SetProperty(ref _newMessageText, value, nameof(NewMessageText));
        }

        public string PeerUsername
        {
            get => _peerUsername;
            set => SetProperty(ref _peerUsername, value, nameof(PeerUsername));
        }

        public string PeerAvatar
        {
            get => _peerAvatar;
            set => SetProperty(ref _peerAvatar, value, nameof(PeerAvatar));
        }

        public string PeerStatus
        {
            get => _peerStatus;
            set => SetProperty(ref _peerStatus, value, nameof(PeerStatus));
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value, nameof(IsBusy));
        }

        public bool IsLoadingMore
        {
            get => _isLoadingMore;
            set => SetProperty(ref _isLoadingMore, value, nameof(IsLoadingMore));
        }

        public ChatViewModel(int userId, int chatId, string peerUsername)
        {
            Directory.CreateDirectory(_cacheDir);
            _userId = userId;
            _chatId = chatId;
            PeerUsername = peerUsername;

            OpenFileCommand = new Command<MessageDto>(async msg => await ExecuteOpenFile(msg));
            SendMessageCommand = new Command(async () => await ExecuteSendMessage());
            AttachFileCommand = new Command(async () => await ExecuteAttachFile());
            LoadMoreCommand = new Command(async () => await LoadMoreMessagesAsync());
        }

        public async Task InitializeAsync()
        {
            LoadMessagesFromCache();
            await LoadMessagesFromServerAsync();
            await ConnectToSignalRAsync();
        }

        public async Task ReconnectAsync()
        {
            if (_hubConnection?.State == HubConnectionState.Disconnected)
            {
                await ConnectToSignalRAsync();
            }
        }

        private string GetCachePath() => Path.Combine(_cacheDir, $"chat_{_chatId}_cache.json");

        public void SaveMessagesToCache()
        {
            var cacheData = new
            {
                LastUpdated = _lastCacheUpdate ?? DateTime.MinValue,
                Messages = Messages.ToList()
            };

            var json = JsonSerializer.Serialize(cacheData);
            File.WriteAllText(GetCachePath(), json);
        }

        public void LoadMessagesFromCache()
        {
            var path = GetCachePath();
            if (!File.Exists(path)) return;

            try
            {
                var json = File.ReadAllText(path);
                var cacheData = JsonSerializer.Deserialize<CacheContainer>(json);

                _lastCacheUpdate = cacheData.LastUpdated;

                Messages.Clear();
                foreach (var msg in cacheData.Messages)
                {
                    Messages.Add(msg);
                    _messagesById[msg.MessageId] = msg;
                    CheckFileCache(msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки кэша: {ex}");
            }
        }

        private async Task LoadMoreMessagesAsync()
        {
            if (IsLoadingMore || !_hasMoreMessages) return;

            try
            {
                IsLoadingMore = true;
                await LoadMessagesFromServerAsync(initialLoad: false);
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        private async Task LoadMessagesFromServerAsync(bool initialLoad = true)
        {
            try
            {
                var url = $"users/chats/{_chatId}/{_userId}/messages?take=100";

                if (initialLoad)
                {
                    url += "&skip=0";
                    if (_lastCacheUpdate.HasValue)
                    {
                        url += $"&since={_lastCacheUpdate.Value:o}";
                    }
                }
                else
                {
                    if (_firstCachedMessageDate.HasValue)
                    {
                        url += $"&before={_firstCachedMessageDate.Value:o}";
                    }
                    else
                    {
                        return; // Не можем загрузить историю без точки отсчета
                    }
                }

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    await ShowAlertAsync("Ошибка",
                        $"Не удалось загрузить сообщения (код {response.StatusCode})");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var messages = JsonSerializer.Deserialize<List<MessageDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (messages == null || messages.Count == 0)
                {
                    if (!initialLoad) _hasMoreMessages = false;
                    return;
                }

                // Обрабатываем сообщения
                ProcessMessages(messages, addToTop: !initialLoad);

                // Обновляем даты для пагинации
                if (initialLoad)
                {
                    _lastCacheUpdate = DateTime.UtcNow;
                    if (Messages.Count > 0)
                    {
                        _firstCachedMessageDate = Messages.Min(m => m.CreatedAt);
                    }
                }
                else if (messages.Count > 0)
                {
                    _firstCachedMessageDate = messages.Min(m => m.CreatedAt);
                }

                // Проверяем, есть ли еще сообщения
                _hasMoreMessages = messages.Count >= 100;

                // Сохраняем в кэш
                SaveMessagesToCache();
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Ошибка",
                    $"При загрузке сообщений произошла ошибка: {ex.Message}");
            }
        }

        private async Task ConnectToSignalRAsync()
        {
            try
            {
                if (_hubConnection != null)
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                }

                // Подключаемся сразу с параметрами
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{HubUrl}?userId={_userId}&chatId={_chatId}")
                    .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2),
                                           TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                    .Build();

                ConfigureHubHandlers();
                await _hubConnection.StartAsync();

                // Эти вызовы можно оставить или убрать — группы уже добавлены на OnConnectedAsync
                await _hubConnection.InvokeAsync("RegisterUser", _userId);
                await _hubConnection.InvokeAsync("JoinChat", _chatId);
                await _hubConnection.InvokeAsync("MarkMessagesAsRead", _chatId, _userId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не удалось подключиться к SignalR-хабу", ex);
            }
        }


        private void ConfigureHubHandlers()
        {
            _hubConnection.On<MessageDto>("ReceiveMessage", async messageDto =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (!_messagesById.ContainsKey(messageDto.MessageId))
                    {
                        CheckFileCache(messageDto);
                        Messages.Add(messageDto);
                        _messagesById[messageDto.MessageId] = messageDto;
                        await _hubConnection.InvokeAsync("MarkMessagesAsRead", _chatId, _userId);

                        // Обновляем кэш при получении нового сообщения
                        _lastCacheUpdate = DateTime.UtcNow;
                        SaveMessagesToCache();
                    }
                });
            });

            _hubConnection.On<StatusDto>("UpdateMessageStatus", statusDto =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_messagesById.TryGetValue(statusDto.MessageId, out var message))
                    {
                        message.Status = statusDto.Status;
                    }
                });
            });

            _hubConnection.On<List<StatusDto>>("BatchUpdateStatuses", statuses =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var st in statuses)
                    {
                        if (_messagesById.TryGetValue(st.MessageId, out var msg))
                        {
                            msg.Status = st.Status;
                        }
                    }
                });
            });

            _hubConnection.Reconnecting += _ =>
            {
                MainThread.BeginInvokeOnMainThread(() => IsBusy = true);
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += _ =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    IsBusy = false;
                    await _hubConnection.InvokeAsync("JoinChat", _chatId);
                    await _hubConnection.InvokeAsync("RegisterUser", _userId);
                });
                return Task.CompletedTask;
            };
        }

        private async Task ExecuteSendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageText)) return;

            try
            {
                await _hubConnection.InvokeAsync("SendMessage",
                    _userId, NewMessageText.Trim(), _chatId, (int?)null);

                NewMessageText = string.Empty;
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Ошибка",
                    $"Не удалось отправить сообщение: {ex.Message}");
            }
        }

        private async Task ExecuteOpenFile(MessageDto msg)
        {
            if (msg?.FileId == null) return;

            try
            {
                var filePath = GetCachedFilePath(msg.FileId.Value, msg.FileName);

                if (!File.Exists(filePath))
                {
                    await DownloadFileAsync(msg.FileId.Value, msg.FileName);
                }

                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Ошибка",
                    $"Не удалось открыть файл: {ex.Message}");
            }
        }

        private async Task ExecuteAttachFile()
        {
            try
            {
                var action = await Application.Current.MainPage.DisplayActionSheet(
                    "Прикрепить файл", "Отмена", null,
                    "Галерея", "Документы", "Камера", "Аудио");

                switch (action)
                {
                    case "Галерея": await PickAndProcessFileAsync(FilePickerFileType.Images); break;
                    case "Документы": await PickAndProcessFileAsync(FilePickerFileType.Pdf); break;
                    case "Камера": await CaptureAndProcessPhotoAsync(); break;
                    case "Аудио": await PickAndProcessAudioAsync(); break;
                }
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Ошибка", ex.Message);
            }
        }

        private async Task PickAndProcessFileAsync(FilePickerFileType fileType)
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = fileType
            });

            if (fileResult != null) await ProcessFileAsync(fileResult);
        }

        private async Task PickAndProcessAudioAsync()
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    [DevicePlatform.Android] = new[] { "audio/*" },
                    [DevicePlatform.iOS] = new[] { "public.audio" },
                    [DevicePlatform.WinUI] = new[] { ".mp3", ".wav", ".ogg" },
                    [DevicePlatform.MacCatalyst] = new[] { ".mp3", ".wav", ".ogg" }
                })
            });

            if (fileResult != null) await ProcessFileAsync(fileResult);
        }

        private async Task CaptureAndProcessPhotoAsync()
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await ShowAlertAsync("Ошибка",
                    "Фотосъемка не поддерживается на этом устройстве");
                return;
            }

            var fileResult = await MediaPicker.Default.CapturePhotoAsync();
            if (fileResult != null) await ProcessFileAsync(fileResult);
        }

        private async Task ProcessFileAsync(FileResult fileResult)
        {
            if (fileResult == null) return;

            try
            {
                IsBusy = true;
                using var fileStream = await fileResult.OpenReadAsync();

                if (fileStream.Length > MaxFileSize)
                {
                    await ShowAlertAsync("Ошибка", "Максимальный размер файла: 16 МБ");
                    return;
                }

                var tempPath = Path.Combine(_cacheDir, $"temp_{Guid.NewGuid()}");
                await using (var tempFile = File.Create(tempPath))
                {
                    await fileStream.CopyToAsync(tempFile);
                }

                using var fileContent = new StreamContent(File.OpenRead(tempPath));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileResult.ContentType);

                using var content = new MultipartFormDataContent();
                content.Add(fileContent, "file", fileResult.FileName);

                var response = await _httpClient.PostAsync(
                    $"users/upload/{_chatId}/{_userId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await ShowAlertAsync("Ошибка", $"Не удалось загрузить файл: {error}");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UploadResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.fileId == 0)
                {
                    await ShowAlertAsync("Ошибка", "Неверный ответ сервера");
                    return;
                }

                var cachedPath = GetCachedFilePath(result.fileId, fileResult.FileName);
                File.Move(tempPath, cachedPath, true);

                // Гарантируем подключение
                if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
                {
                    await ConnectToSignalRAsync();
                }

                // Отправляем сообщение о файле
                await _hubConnection.InvokeAsync("SendFileMessage",
                    _userId, result.fileId, _chatId);
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Ошибка", $"Ошибка обработки файла: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ProcessMessages(List<MessageDto> newMessages, bool addToTop = false)
        {
            if (newMessages == null || newMessages.Count == 0) return;

            // Для подгрузки истории (в начало)
            if (addToTop)
            {
                // Сохраняем первый видимый элемент перед изменением
                _firstVisibleMessageBeforeLoad = Messages.FirstOrDefault();

                // Вставляем сообщения в начало коллекции
                foreach (var msg in newMessages.OrderBy(m => m.CreatedAt))
                {
                    if (_messagesById.ContainsKey(msg.MessageId)) continue;

                    Messages.Insert(0, msg);
                    _messagesById[msg.MessageId] = msg;
                    CheckFileCache(msg);
                }
            }
            // Для новых сообщений (в конец)
            else
            {
                foreach (var msg in newMessages)
                {
                    if (_messagesById.TryGetValue(msg.MessageId, out var existing))
                    {
                        // Обновляем существующее сообщение
                        var index = Messages.IndexOf(existing);
                        Messages[index] = msg;
                    }
                    else
                    {
                        // Добавляем новое сообщение
                        Messages.Add(msg);
                    }

                    _messagesById[msg.MessageId] = msg;
                    CheckFileCache(msg);
                }
            }
        }

        public static void ClearOldCache()
        {
            var cacheDir = Path.Combine(FileSystem.CacheDirectory, "MessengerFiles");
            if (!Directory.Exists(cacheDir)) return;

            var threshold = DateTime.Now.AddDays(-14);
            foreach (var file in Directory.EnumerateFiles(cacheDir))
            {
                try
                {
                    var info = new FileInfo(file);
                    if (info.LastWriteTime < threshold)
                        info.Delete();
                }
                catch
                {
                    Debug.WriteLine("Ошибка при очистке кэша приложения");
                }
            }
        }


        private void CheckFileCache(MessageDto message)
        {
            if (message?.FileId == null || string.IsNullOrEmpty(message.FileName))
                return;

            var cachedPath = GetCachedFilePath(message.FileId.Value, message.FileName);

            if (File.Exists(cachedPath))
            {
                message.LocalPath = cachedPath;
            }
            else
            {
                _ = Task.Run(async () =>
                {
                    await DownloadFileAsync(message.FileId.Value, message.FileName);
                    message.LocalPath = cachedPath;
                });
            }
        }

        private async Task DownloadFileAsync(int fileId, string fileName)
        {
            try
            {
                var cachedPath = GetCachedFilePath(fileId, fileName);
                if (File.Exists(cachedPath)) return;

                var response = await _httpClient.GetAsync($"users/{fileId}");
                response.EnsureSuccessStatusCode();

                await using var fs = File.Create(cachedPath);
                await response.Content.CopyToAsync(fs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки файла: {ex}");
            }
        }

        private string GetCachedFilePath(int fileId, string fileName)
        {
            // Получаем только валидные символы
            var invalid = Path.GetInvalidFileNameChars();
            var cleanName = new string(fileName.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());

            if (cleanName.Length > 100)
                cleanName = cleanName.Substring(cleanName.Length - 100);

            return Path.Combine(_cacheDir, $"{fileId}_{cleanName}");
        }

        private async Task ShowAlertAsync(string title, string message)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }

        private void SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private class UploadResult
        {
            public int fileId { get; set; }
            public string url { get; set; } = string.Empty;
        }

        private class CacheContainer
        {
            public DateTime LastUpdated { get; set; }
            public List<MessageDto> Messages { get; set; }
        }
    }
}