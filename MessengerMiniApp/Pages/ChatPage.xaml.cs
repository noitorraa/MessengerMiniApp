using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using MessengerServer.Model;
using System.Collections.ObjectModel;

namespace MessengerMiniApp.Pages
{
    public partial class ChatPage : ContentPage
    {
        private HubConnection _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-7295.twc1.net/api/users/";
        private readonly int _userId;
        private readonly int _chatId;
        private ObservableCollection<string> _messages;

        public ChatPage(int userId, int chatId)
        {
            InitializeComponent();
            _userId = userId;
            _chatId = chatId;
            _messages = new ObservableCollection<string>();
            MessagesCollectionView.ItemsSource = _messages;
            LoadMessages();
            _ = ConnectToSignalR(); // Используем дискорд для асинхронного вызова
        }

        private async void LoadMessages()
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}chats/{_chatId}/messages");
            if (response.IsSuccessStatusCode)
            {
                var messages = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());
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
                await _hubConnection.InvokeAsync("SendMessage", _userId, MessageEntry.Text, _chatId);
                MessageEntry.Text = string.Empty;
            }
        }

        private async Task ConnectToSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://noitorraa-messengerserver-7295.twc1.net/chatHub")
                .Build();

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(5000);
                await ConnectToSignalR();
            };

            // Подписка на получение сообщений
            _hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _messages.Add(message); // Добавление строки в коллекцию
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinChat", _chatId); // Вход в группу
                Console.WriteLine("Успешное подключение к чату");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }
    }
}
