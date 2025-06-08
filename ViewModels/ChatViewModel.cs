// ViewModels/ChatViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.AspNetCore.SignalR.Client;
using MessengerMiniApp.DTOs;

namespace MessengerMiniApp.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        // Для HTTP-запросов к API
        private readonly HttpClient _httpClient;
        private string _cacheDir = Path.Combine(FileSystem.CacheDirectory, "MessengerFiles");

        // SignalR-хаб
        private HubConnection _hubConnection;

        // ID текущего пользователя и ID чата
        private readonly int _userId;
        private readonly int _chatId;

        // Словарь для быстрого поиска сообщения по MessageId
        private readonly Dictionary<int, MessageDto> _messagesById;

        // Коллекция сообщений
        public ObservableCollection<MessageDto> Messages { get; }

        // Текст нового сообщения, привязанный к Entry.Text
        private string _newMessageText;
        public string NewMessageText
        {
            get => _newMessageText;
            set
            {
                if (_newMessageText != value)
                {
                    _newMessageText = value;
                    OnPropertyChanged(nameof(NewMessageText));
                }
            }
        }

        private string _peerUsername;
        public string PeerUsername
        {
            get => _peerUsername;
            set
            {
                if (_peerUsername != value)
                {
                    _peerUsername = value;
                    OnPropertyChanged(nameof(PeerUsername));
                }
            }
        }

        private string _peerAvatar; // тут будет URL или локальный путь к картинке
        public string PeerAvatar
        {
            get => _peerAvatar;
            set
            {
                if (_peerAvatar != value)
                {
                    _peerAvatar = value;
                    OnPropertyChanged(nameof(PeerAvatar));
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged()
        {
            throw new NotImplementedException();
        }

        private string _peerStatus;
        public string PeerStatus
        {
            get => _peerStatus;
            set
            {
                if (_peerStatus != value)
                {
                    _peerStatus = value;
                    OnPropertyChanged(nameof(PeerStatus));
                }
            }
        }

        // Команды
        public ICommand SendMessageCommand { get; }
        public ICommand AttachFileCommand { get; }
        public ICommand DownloadFileCommand { get; }

        // Текущее UserId (чтобы в XAML передать в конвертеры)
        public int CurrentUserId => _userId;

        // Адреса API и SignalR. Можно вынести в конфиг.
        private const string BaseApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/";
        private const string HubUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/chatHub";

        public ChatViewModel(int userId, int chatId, string peerUsername)
        {
            Directory.CreateDirectory(_cacheDir);
            _userId = userId;
            _chatId = chatId;

            PeerUsername = peerUsername;    // например, получить из API или со страницы чатов (там мы получаем название чата и его по сути можно и отобразить здесь)
            PeerStatus = "Не в сети";       // или «отсутствует» и т. п. (пока не реализованы статусы, поэтому просот заглушка)

            _messagesById = new Dictionary<int, MessageDto>();
            Messages = new ObservableCollection<MessageDto>();

            // Инициализируем HttpClient
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseApiUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Команды
            SendMessageCommand = new Command(async () => await ExecuteSendMessage());
            AttachFileCommand = new Command(async () => await ExecuteAttachFile());
            DownloadFileCommand = new Command<string>(async (url) => await ExecuteDownloadFile(url));
        }

        /// <summary>
        /// Основной метод, который нужно вызвать из Page.OnAppearing:
        /// загружает историю (LoadMessages) и подключается к SignalR.
        /// </summary>
        public async Task InitializeAsync()
        {
            await LoadMessagesAsync();
            await ConnectToSignalRAsync();
        }

        /// <summary>
        /// Загружает все существующие сообщения чата из API.
        /// </summary>
        private async Task LoadMessagesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/chats/{_chatId}/{_userId}/messages");
                if (!response.IsSuccessStatusCode)
                {
                    // Показываем Alert и возвращаемся
                    await Application.Current!.MainPage.DisplayAlert("Ошибка",
                        $"Не удалось загрузить сообщения (код {response.StatusCode})", "OK");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var messages = JsonSerializer.Deserialize<List<MessageDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (messages == null) return;

                // Очищаем текущие
                Messages.Clear();
                _messagesById.Clear();

                foreach (var msg in messages.OrderBy(m => m.CreatedAt))
                {
                    Messages.Add(msg);
                    _messagesById[msg.MessageId] = msg;
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage.DisplayAlert("Ошибка",
                    $"При загрузке сообщений произошла ошибка: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Подключается к SignalR-хабу с автоматическим реконнектом.
        /// Подписывается на события: ReceiveMessage, BatchUpdateStatuses, RefreshMessages.
        /// </summary>
        private async Task ConnectToSignalRAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(HubUrl)
                .WithAutomaticReconnect(new[] {
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
                })
                .Build();

            _hubConnection.On<MessageDto>("ReceiveMessage", (messageDto) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    // Простая обработка новых сообщений
                    if (!_messagesById.ContainsKey(messageDto.MessageId))
                    {
                        Messages.Add(messageDto);
                        _messagesById[messageDto.MessageId] = messageDto;
                        await _hubConnection.InvokeAsync("MarkMessagesAsRead", _chatId, _userId);
                    }
                });
            });

            _hubConnection.On<StatusDto>("UpdateMessageStatus", (statusDto) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_messagesById.TryGetValue(statusDto.MessageId, out var message))
                    {
                        message.Status = statusDto.Status;
                    }
                });
            });

            _hubConnection.On<List<StatusDto>>("BatchUpdateStatuses", (statuses) =>
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

            _hubConnection.Reconnecting += ex =>
            {
                MainThread.BeginInvokeOnMainThread(() => IsBusy = true);
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    IsBusy = false;
                    await _hubConnection.InvokeAsync("JoinChat", _chatId);
                    await _hubConnection.InvokeAsync("RegisterUser", _userId);
                });
                return Task.CompletedTask;
            };

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("RegisterUser", _userId);
                await _hubConnection.InvokeAsync("JoinChat", _chatId);
                await _hubConnection.InvokeAsync("MarkMessagesAsRead", _chatId, _userId);
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage.DisplayAlert("Ошибка подключения",
                    $"Не удалось подключиться к чату: {ex.Message}", "Повторить");
                // Автоматический реконнект через 5 секунд
                await Task.Delay(5000);
                await ConnectToSignalRAsync();
            }
        }


        /// <summary>
        /// Отправка обычного текстового сообщения через SignalR.
        /// </summary>
        private async Task ExecuteSendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageText)) return;

            try
            {
                // Вызываем метод на хабе: SendMessage(userId, content, chatId, null)
                await _hubConnection.InvokeAsync("SendMessage", _userId, NewMessageText.Trim(), _chatId, (int?)null);
                NewMessageText = string.Empty;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage.DisplayAlert("Ошибка", $"Не удалось отправить сообщение: {ex.Message}", "OK");
            }
        }

        public async Task ReconnectAsync()
        {
            if (_hubConnection?.State == HubConnectionState.Disconnected)
            {
                await ConnectToSignalRAsync();
            }
        }

        /// <summary>
        /// Прикрепление файла: открываем файловый диалог, отправляем на сервер, а затем через SignalR — сообщение с FileId.
        /// </summary>
        private async Task ExecuteAttachFile()
        {
            try
            {
                var action = await Application.Current.MainPage.DisplayActionSheet(
                    "Прикрепить файл",
                    "Отмена",
                    null,
                    "Галерея",
                    "Документы",
                    "Камера",
                    "Аудио"
                );

                switch (action)
                {
                    case "Галерея":
                        await PickGalleryAsync();
                        break;

                    case "Документы":
                        await PickFileAsync();
                        break;

                    case "Камера":
                        await TakePhotoAsync();
                        break;

                    case "Аудио":
                        await PickAudioAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Ошибка: {ex.Message}", "OK");
            }
        }

        private async Task PickGalleryAsync()
        {
            try
            {
                FileResult fileResult = null;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    fileResult = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Выберите фото",
                        FileTypes = FilePickerFileType.Images
                    });
                });

                if (fileResult != null)
                {
                    await ProcessFileAsync(fileResult);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Ошибка при выборе фото: {ex.Message}", "OK");
            }
        }

        private async Task PickFileAsync()
        {
            try
            {
                FileResult fileResult = null;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    fileResult = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Выберите документ",
                        FileTypes = FilePickerFileType.Pdf
                    });
                });

                if (fileResult != null)
                {
                    await ProcessFileAsync(fileResult);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Ошибка при выборе документа: {ex.Message}", "OK");
            }
        }

        private async Task TakePhotoAsync()
        {
            try
            {
                FileResult fileResult = null;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {
                        fileResult = await MediaPicker.Default.CapturePhotoAsync();
                    }
                });

                if (fileResult != null)
                {
                    await ProcessFileAsync(fileResult);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        "Фотосъемка не поддерживается на этом устройстве", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Ошибка при съемке фото: {ex.Message}", "OK");
            }
        }

        private async Task PickAudioAsync()
        {
            try
            {
                FileResult fileResult = null;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    fileResult = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Выберите аудиофайл",
                        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "audio/*" } },
                    { DevicePlatform.iOS, new[] { "public.audio" } },
                    { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".ogg" } },
                    { DevicePlatform.MacCatalyst, new[] { ".mp3", ".wav", ".ogg" } }
                })
                    });
                });

                if (fileResult != null)
                {
                    await ProcessFileAsync(fileResult);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Ошибка при выборе аудио: {ex.Message}", "OK");
            }
        }

        private async Task ProcessFileAsync(FileResult fileResult)
        {
            try
            {
                // Показываем индикатор загрузки
                IsBusy = true;

                using var fileStream = await fileResult.OpenReadAsync();

                // Проверка размера файла
                if (fileStream.Length > 16 * 1024 * 1024)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        "Максимальный размер файла: 16 МБ", "OK");
                    return;
                }

                // Создаем контент для отправки
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(fileStream);

                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                content.Add(fileContent, "file", fileResult.FileName);

                // Отправка файла на сервер
                var response = await _httpClient.PostAsync($"users/upload/{_chatId}/{_userId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        $"Не удалось загрузить файл: {error}", "OK");
                    return;
                }

                // Обработка ответа сервера
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UploadResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.fileId == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        "Неверный ответ сервера при загрузке файла", "OK");
                    return;
                }

                // Сохраняем файл локально
                var localPath = Path.Combine(_cacheDir, fileResult.FileName);
                using var localFileStream = File.Create(localPath);
                await fileStream.CopyToAsync(localFileStream);

                // Отправка сообщения с файлом через SignalR
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("SendFileMessage", _userId, result.fileId, _chatId);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        "Нет подключения к чату", "OK");
                }
            }
            finally
            {
                IsBusy = false; // Скрываем индикатор
            }
        }


        /// <summary>
        /// По нажатию по файлу загружаем файл на устройство
        /// </summary>
        private async Task ExecuteDownloadFile(string fileUrl) // файл передаётся напрямую, не ссылкой!
        {
            var fileName = Path.GetFileName(fileUrl);
            var localPath = Path.Combine(_cacheDir, fileName);

            // Проверяем, существует ли файл локально
            if (File.Exists(localPath))
            {
                // Открываем файл
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(localPath)
                });
                return;
            }

            // Скачиваем файл
            var response = await _httpClient.GetAsync(fileUrl);
            await using (var fs = File.Create(localPath))
            {
                await response.Content.CopyToAsync(fs);
            }

            // Открываем файл
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(localPath)
            });
        }


        public static void ClearCacheOnStartup()
        {
            var cacheDir = Path.Combine(FileSystem.CacheDirectory, "MessengerFiles");
            if (Directory.Exists(cacheDir))
            {
                Directory.Delete(cacheDir, true);
            }
            Directory.CreateDirectory(cacheDir);
        }

        // Класс для десериализации ответа upload API
        private class UploadResult
        {
            public int fileId { get; set; }
            public string url { get; set; } = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
