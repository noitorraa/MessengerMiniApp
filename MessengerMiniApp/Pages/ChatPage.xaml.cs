using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using MessengerServer.Model;
using System.Collections.ObjectModel;

namespace MessengerMiniApp.Pages
{
    public partial class ChatPage : ContentPage
    {
        private HubConnection testConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-7295.twc1.net/api/users/";
        private readonly int _userId;
        private readonly int _chatId;
        private ObservableCollection<MessageDto> _messages;

        public ChatPage(int userId, int chatId)
        {
            InitializeComponent();
            _userId = userId;
            _chatId = chatId;
            _messages = new ObservableCollection<MessageDto>();
            MessagesCollectionView.ItemsSource = _messages;
            LoadMessages();
            _ = ConnectToSignalR(); // Используем дискорд для асинхронного вызова
        }

        private async void LoadMessages()
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}chats/{_chatId}/messages");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var messages = JsonConvert.DeserializeObject<List<MessageDto>>(json);

                foreach (var message in messages)
                {
                    _messages.Add(message);
                }
            }
        }

        private async Task ConnectToSignalR()
        {
            testConnection = new HubConnectionBuilder()
                .WithUrl("https://noitorraa-messengerserver-7295.twc1.net/chatHub", options =>
                {
                    options.Headers["UserId"] = _userId.ToString();
                })
                .Build();

            // Обработчик закрытия подключения
            testConnection.Closed += async (error) =>
            {
                await Task.Delay(5000);
                await ConnectToSignalR();
            };

            testConnection.On<string>("ReceiveTest", msg => {
                Console.WriteLine($"TEST MESSAGE: {msg}");
            });

            await testConnection.StartAsync();
            await testConnection.InvokeAsync("JoinGroup", "test_group");
            await testConnection.InvokeAsync("SendToGroup", "test_group", "Hello World");
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            await testConnection.InvokeAsync("SendMessage", _userId, MessageEntry.Text, _chatId);
            MessageEntry.Text = string.Empty;
        }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string SenderName { get; set; }
    }
}
