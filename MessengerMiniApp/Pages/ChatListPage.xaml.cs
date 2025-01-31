using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using MessengerServer.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerMiniApp.Pages
{
    public partial class ChatListPage : ContentPage
    {
        private HubConnection _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-7295.twc1.net/api/users/";
        private readonly int _userId;
        private ObservableCollection<Chat> _chats;

        public ChatListPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _chats = new ObservableCollection<Chat>();
            chatListView.ItemsSource = _chats;
            LoadChats();
            _ = ConnectToSignalR(); // Используем дискорд для асинхронного вызова
        }

        private async void LoadChats()
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}chats/{_userId}");
            if (response.IsSuccessStatusCode)
            {
                var chats = JsonConvert.DeserializeObject<List<Chat>>(await response.Content.ReadAsStringAsync());
                foreach (var chat in chats)
                {
                    _chats.Add(chat);
                }
            }
        }

        private async Task ConnectToSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://noitorraa-messengerserver-7295.twc1.net/chatHub", options =>
                {
                    options.Headers["UserId"] = _userId.ToString();
                })
                .Build();

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(5000);
                await ConnectToSignalR();
            };

            _hubConnection.On<Chat>("ReceiveNewChat", (chat) => // нужно добавить событие на сервере
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _chats.Add(chat);
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine($"Состояние подключения: {_hubConnection.State}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            var searchQuery = searchBar.Text;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await DisplayAlert("Ошибка", "Введите логин", "OK");
                return;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{ApiUrl}search?login={searchQuery}");
                if (response.IsSuccessStatusCode)
                {
                    var users = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
                    if (users.Any())
                    {
                        await Navigation.PushAsync(new SearchResultsPage(users, _userId));
                    }
                    else
                    {
                        await DisplayAlert("Результаты", "Пользователи не найдены", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось выполнить поиск", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async void OnChatTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Chat chat)
            {
                await Navigation.PushAsync(new ChatPage(_userId, chat.ChatId));
            }
        }
    }
}
