using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using MessengerServer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerMiniApp.Pages
{
    public partial class ChatListPage : ContentPage
    {
        private HubConnection? _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";
        private readonly int _userId;
        private ObservableCollection<ChatDto> _chats;

        public ChatListPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _chats = new ObservableCollection<ChatDto>();
            chatListView.ItemsSource = _chats;
            _ = LoadChats();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ConnectToSignalR();
            await LoadChats();
        }

        private async Task LoadChats()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiUrl}chats/{_userId}");
                if (response.IsSuccessStatusCode)
                {
                    var chats = JsonConvert.DeserializeObject<List<ChatDto>>(
                        await response.Content.ReadAsStringAsync()
                    );

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _chats.Clear();
                        foreach (var chat in chats)
                        {
                            chat.ChatName = GetChatName(chat.Members ?? new List<UserDto>(), _userId);
                            _chats.Add(chat);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

private string GetChatName(List<UserDto>? members, int currentUserId)
{
    if (members == null || members.Count == 0)
        return "Неизвестный чат";

    if (members.Count == 1)
        return "Произошла ошибка при создании чата, удалите чат и создайте заново";

    if (members.Count == 2)
    {
        var user = members.FirstOrDefault(u => u?.UserId != currentUserId);
        return user?.Username ?? string.Empty;
    }

    return string.Join(", ", members.Where(u => u != null).Select(u => u.Username ?? string.Empty));
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

            _hubConnection.On("NotifyUpdateChatList", async () =>
            {
                Console.WriteLine("Обновляем список чатов, потому что получили уведомление");
                await LoadChats();
            });

            try
            {
                if (_hubConnection != null)
                {
                    await _hubConnection.StartAsync();
                    Console.WriteLine($"SignalR: {_hubConnection.State}");
                    await _hubConnection.InvokeAsync("RegisterUser", _userId);
                    Console.WriteLine($"Registered in group user_{_userId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка signalR: {ex.Message}");
            }
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            var searchQuery = searchBar.Text;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await DisplayAlert("Ошибка", "Введите искомый логин", "OK");
                return;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{ApiUrl}search?login={searchQuery}");
                if (response.IsSuccessStatusCode)
                {
                    var users = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
                    if (users != null && users.Any())
                    {
                        await Navigation.PushAsync(new SearchResultsPage(users, _userId));
                    }
                    else
                    {
                        await DisplayAlert("Пользователь не найден", "Удостоверьтесь в правильности написания логина попробуйте заново", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "На сервере произошла ошибка при поиске пользователя", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async void OnChatTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is ChatDto selectedChat)
            {
                if (selectedChat == null)
                {
                    await DisplayAlert("Ошибка", "Выбранный чат не найден", "OK");
                    return;
                }
                await Navigation.PushAsync(new ChatPage(_userId, selectedChat.ChatId));
            }

            chatListView.SelectedItem = null;
        }

        private async void OnDeleteChat(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;
            var chat = swipeItem?.BindingContext as ChatDto;

            if (chat == null)
            {
                await DisplayAlert("Ошибка", "Чат не найден", "OK");
                return;
            }

            bool confirm = await DisplayAlert("Удаление чата", $"Вы уверены, что хотите удалить чат с {chat.ChatName}?", "Да", "Нет");
            if (!confirm)
                return;

            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiUrl}chats/{chat.ChatId}");
                if (response.IsSuccessStatusCode)
                {
                    _chats.Remove(chat);
                    await DisplayAlert("Успешно", "Чат удалён", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось удалить чат", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }
    }

    public class ChatDto
    {
        public int ChatId { get; set; }
        public string? ChatName { get; set; }
        public List<UserDto>? Members { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
    }
}
