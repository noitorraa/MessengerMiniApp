using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MessengerMiniApp.Pages
{
    public partial class ChatPage : ContentPage
    {
        private HubConnection _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-ba27.twc1.net/api/users/";
        private readonly int _userId;
        public int CurrentUserId => _userId;
        private readonly int _chatId;
        private ObservableCollection<MessageDto> _messages;

        public ChatPage(int userId, int chatId)
        {
            InitializeComponent();
            _userId = userId;
            _chatId = chatId;
            _messages = new ObservableCollection<MessageDto>();
            MessagesCollectionView.ItemsSource = _messages;
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
            BindingContext = this;
            LoadMessages();

            _ = ConnectToSignalR(); // ���������� ������� ��� ������������ ������ (await ������ ������ ��� ����� �� �����������)
        }

        private async Task MarkMessagesAsRead()
        {
            var unreadMessages = _messages
                .Where(m => !m.IsRead && m.UserID != _userId)
                .ToList();

            if (unreadMessages.Any())
            {
                var messageIds = unreadMessages.Select(m => m.MessageId).ToList();
                await _hubConnection.InvokeAsync("UpdateMessageStatusBatch", messageIds, _userId);
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
                foreach (var message in messages)
                {
                    Console.WriteLine($"��������� ID: {message.MessageId}, IsRead: {message.IsRead}, UserID: {message.UserID}");
                    _messages.Add(message); // IsRead ��� �������� � �������
                }
                await MarkMessagesAsRead(); // ��������� ������ ���������, ����� ��� ���������
            }
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageEntry.Text))
            {
                await _hubConnection.InvokeAsync("SendMessage", _userId, MessageEntry.Text, _chatId);
                MessageEntry.Text = string.Empty;
            }
        }

        private async Task ConnectToSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://noitorraa-messengerserver-ba27.twc1.net/chatHub")
                .Build();

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(5000);
                await ConnectToSignalR();
            };

            // �������� �� ��������� ���������
            _hubConnection.On<string, int, int>("ReceiveMessage", (content, senderId, messageId) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _messages.Add(new MessageDto
                    {
                        MessageId = messageId, // ��������� MessageId
                        Content = content,
                        UserID = senderId,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = senderId != _userId // ��� ����������� ������ "���������"
                    });
                });
            });

            _hubConnection.On<List<int>, int>("ReceiveMessageStatusUpdate", (messageIds, userId) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var messageId in messageIds)
                    {
                        var message = _messages.FirstOrDefault(m => m.MessageId == messageId);
                        if (message != null && message.UserID != userId)
                        {
                            message.IsRead = true; // ��������� ������ ��������� ��� �����������
                        }
                    }
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinChat", _chatId); // ���� � ������
                Console.WriteLine("�������� ����������� � ����");
                await MarkMessagesAsRead();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �����������: {ex.Message}");
            }
        }

        private void OnAttachClicked(object sender, EventArgs e) // ���������� ����
        {

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

        private bool _isRead;
        [JsonProperty("isRead")]
        public bool IsRead
        {
            get => _isRead;
            set
            {
                _isRead = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
