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

        // Команды
        public ICommand SendMessageCommand { get; }
        public ICommand AttachFileCommand { get; }
        public ICommand DownloadFileCommand { get; }

        // Текущее UserId (чтобы в XAML передать в конвертеры)
        public int CurrentUserId => _userId;

        // Адреса API и SignalR. Можно вынести в конфиг.
        private const string BaseApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/";
        private const string HubUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/chatHub";

        public ChatViewModel(int userId, int chatId)
        {
            _userId = userId;
            _chatId = chatId;

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

                // Добавляем в коллекцию (они сортированы на сервере по CreatedAt? Иначе отсортируйте здесь)
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
                // Если уже было подключение, корректно остановим его
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(HubUrl)
                .WithAutomaticReconnect() // Встроенный механизм реконнекта
                .Build();

            // При получении одиночного сообщения
            _hubConnection.On<MessageDto>("ReceiveMessage", (messageDto) =>
            {
                // SignalR может вызывать callback не в UI-потоке, поэтому обёртка
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Если файл — просто заменим Content заглушкой (имя файла)
                    if (!string.IsNullOrWhiteSpace(messageDto.FileUrl))
                    {
                        messageDto.Content = $"[Файл: {System.IO.Path.GetFileName(messageDto.FileUrl)}]";
                    }

                    // Добавляем в коллекцию и словарь
                    Messages.Add(messageDto);
                    _messagesById[messageDto.MessageId] = messageDto;
                });
            });

            // При пакетном обновлении статусов (Delivery/Read)
            _hubConnection.On<List<StatusDto>>("BatchUpdateStatuses", (statuses) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var st in statuses)
                    {
                        if (_messagesById.TryGetValue(st.MessageId, out var msg))
                        {
                            msg.Status.Status = st.Status;
                        }
                    }
                });
            });

            // При команде RefreshMessages — перечитать всю историю
            _hubConnection.On("RefreshMessages", () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadMessagesAsync();
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                // После подключения сразу входим в группу чата
                await _hubConnection.InvokeAsync("JoinChat", _chatId);
                // И помечаем все сообщения как прочитанные
                await _hubConnection.InvokeAsync("MarkMessagesAsRead", _chatId, _userId);
            }
            catch (Exception ex)
            {
                // Если не удалось подключиться, покажем уведомление (не фатально)
                await Application.Current!.MainPage.DisplayAlert("Ошибка",
                    $"Не удалось подключиться к чату: {ex.Message}", "OK");
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

        /// <summary>
        /// Прикрепление файла: открываем файловый диалог, отправляем на сервер, а затем через SignalR — сообщение с FileId.
        /// </summary>
        private async Task ExecuteAttachFile()
        {
            try
            {
                var fileResult = await FilePicker.Default.PickAsync();
                if (fileResult == null) return;

                using var stream = await fileResult.OpenReadAsync();
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileResult.ContentType);
                content.Add(fileContent, "file", fileResult.FileName);

                var response = await _httpClient.PostAsync($"users/upload/{_chatId}/{_userId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current!.MainPage.DisplayAlert("Ошибка", "Не удалось прикрепить файл", "OK");
                    return;
                }

                // Ожидаем, что сервер вернёт JSON вида { fileId: 123, url: "https://..." }
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UploadResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null && result.fileId != 0)
                {
                    // Отправляем команду на хаб, чтобы все пользователи получили сообщение с файлом
                    await _hubConnection.InvokeAsync("SendFileMessage", _userId, result.fileId, _chatId);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage.DisplayAlert("Ошибка", $"Прикрепление файла не удалось: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Открываем переданный URL (fileUrl) внешним браузером/менеджером.
        /// </summary>
        private async Task ExecuteDownloadFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            try
            {
                await Launcher.OpenAsync(new Uri(fileUrl));
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage.DisplayAlert("Ошибка", $"Не удалось открыть файл: {ex.Message}", "OK");
            }
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
