using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Plugin.Maui.Audio;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MessengerMiniApp.Pages
{
    public partial class ChatPage : ContentPage
    {
        public ICommand DownloadFileCommand { get; }
        private HubConnection _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";
        private readonly int _userId;
        public int CurrentUserId => _userId;
        private readonly int _chatId;
        private ObservableCollection<MessageDto> _messages;

        public ChatPage(int userId, int chatId)
        {
            InitializeComponent();
            DownloadFileCommand = new Command<string>(DownloadFile);
            _userId = userId;
            _chatId = chatId;
            _messages = new ObservableCollection<MessageDto>();
            MessagesCollectionView.ItemsSource = _messages;
            var nullToVisibilityConverter = new NullToVisibilityConverter();
            var statusToColorConverter = new StatusToColorConverter();
            var statusToTextConverter = new StatusToTextConverter();
            var isCurrentUserConverter = new IsCurrentUserConverter { CurrentUserId = _userId };
            var isOutgoingMessageConverter = new IsOutgoingMessageConverter { CurrentUserId = _userId };
            var userIdToColorConverter = new UserIdToColorConverter { CurrentUserId = _userId };
            Resources.Add("StatusToColorConverter", statusToColorConverter);
            Resources.Add("StatusToTextConverter", statusToTextConverter);
            Resources.Add("IsCurrentUserConverter", isCurrentUserConverter);
            Resources.Add("IsOutgoingMessageConverter", isOutgoingMessageConverter);
            Resources.Add("UserIdToColorConverter", userIdToColorConverter);
            Resources.Add("NullToVisibilityConverter", nullToVisibilityConverter);
            BindingContext = this;
            LoadMessages();

            _ = ConnectToSignalR(); // Используем дискорд для асинхронного вызова (await нельзя потому что метод не асинхронный)
        }

        private async void DownloadFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            try
            {
                // Открываем файл в браузере или скачиваем
                await Launcher.OpenAsync(new Uri(fileUrl));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось скачать файл: {ex.Message}", "OK");
            }
        }


        private async void LoadMessages()
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}chats/{_chatId}/{_userId}/messages");
            if (response.IsSuccessStatusCode)
            {
                var messages = JsonConvert.DeserializeObject<List<MessageDto>>(
                    await response.Content.ReadAsStringAsync()
                );
                _messages.Clear();
                foreach (var message in messages)
                {
                    _messages.Add(message);
                }
            }
        }


        private async void OnSendClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageEntry.Text))
            {
                await _hubConnection.InvokeAsync("SendMessage", _userId, MessageEntry.Text, _chatId, null);
                MessageEntry.Text = string.Empty;
            }
        }


        private async Task ConnectToSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://noitorraa-messengerserver-c2cc.twc1.net/chatHub")
                .Build();

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(5000);
                await ConnectToSignalR();
            };

            // Подписка на получение сообщений
            _hubConnection.On<MessageDto>("ReceiveMessage", (messageDto) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (messageDto.FileUrl != null)
                    {
                        messageDto.Content = $"[Файл: {Path.GetFileName(messageDto.FileUrl)}]";
                    }
                    _messages.Add(messageDto);
                });
            });

            _hubConnection.On<int, int>("UpdateMessageStatus", (messageId, status) =>
            {
                var message = _messages.FirstOrDefault(m => m.MessageId == messageId);
                if (message != null)
                {
                    message.Status = status;
                }
            });

            _hubConnection.On("RefreshMessages", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Перезагрузить сообщения или обновить конкретные статусы
                    Console.WriteLine("Перезагружаем страницу");
                    LoadMessages();
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinChat", _chatId); // Вход в группу
                Console.WriteLine("Успешное подключение к чату");
                Console.WriteLine("Отмечаем сообщения как прочитанные");
                await _hubConnection.InvokeAsync("MarkMessagesAsRead", _chatId, _userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }

        private async void OnAttachClicked(object sender, EventArgs e)
        {
            var fileResult = await FilePicker.Default.PickAsync();
            if (fileResult == null) return; 

            using var stream = await fileResult.OpenReadAsync();
            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileResult.ContentType);
            content.Add(fileContent, "file", fileResult.FileName);

            var response = await _httpClient.PostAsync(
                $"{ApiUrl}upload/{_chatId}/{_userId}",
                content);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeAnonymousType(
                    await response.Content.ReadAsStringAsync(),
                    new { fileId = 0, url = "" });

                await _hubConnection.InvokeAsync("SendFileMessage",
                    _userId,
                    result.fileId,
                    _chatId);
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить файл", "OK");
            }
        }

    }

    public class MessageDto : INotifyPropertyChanged
    {
        [JsonProperty("messageId")]
        public int MessageId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("fileId")]
        public int? FileId { get; set; }

        [JsonProperty("fileType")]
        public string? FileType { get; set; }

        [JsonProperty("fileUrl")]
        public string? FileUrl { get; set; }
        private int _status;

        [JsonProperty("status")]
        public int Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(); // Важно: вызывает обновление UI
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
