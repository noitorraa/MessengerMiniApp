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
        private HubConnection _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-ba27.twc1.net/api/users/";
        private readonly int _userId;
        private ObservableCollection<ChatDto> _chats;

        public ChatListPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _chats = new ObservableCollection<ChatDto>();
            chatListView.ItemsSource = _chats;
            App.signalRService.OnChatListUpdated += OnChatListUpdated;
            _ = LoadChats();
        }

        private async void OnChatListUpdated()
        {
            Console.WriteLine("Получен сигнал для обновления списка чатов");
            await LoadChats();
        }

        protected override void OnDisappearing()
        {
            // Отписка при уходе со страницы
            App.signalRService.OnChatListUpdated -= OnChatListUpdated;
            base.OnDisappearing();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ConnectToSignalR();
            await LoadChats(); // Обновляем список чатов при каждом появлении страницы
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
                            // Формируем название на клиенте
                            chat.ChatName = GetChatName(chat.Members, _userId);
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

        private string GetChatName(List<UserDto> members, int currentUserId)
        {
            if (members.Count == 1) return "Личный чат (ошибка)";

            // Для личных чатов: имя собеседника
            if (members.Count == 2)
            {
                return members.First(u => u.UserId != currentUserId).Username;
            }

            // Для групповых чатов: список всех участников
            return string.Join(", ", members.Select(u => u.Username));
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

            _hubConnection.On("NotifyUpdateChatList", async () =>
            {
                Console.WriteLine("Получен сигнал для обновления списка чатов");
                await LoadChats(); // Загружаем обновленный список чатов
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

        private async void OnChatTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is ChatDto selectedChat)
            {
                // Переходим на страницу чата
                await Navigation.PushAsync(new ChatPage(_userId, selectedChat.ChatId));
            }

            // Сбрасываем выделение
            chatListView.SelectedItem = null;
        }

        private async void OnDeleteChat(object sender, EventArgs e)
        {
            // Получаем элемент, который был свайпнут
            var swipeItem = sender as SwipeItem;
            var chat = swipeItem.BindingContext as ChatDto;

            if (chat == null)
                return;

            // Подтверждение удаления
            bool confirm = await DisplayAlert("Удаление чата", $"Вы уверены, что хотите удалить чат {chat.ChatName}?", "Да", "Нет");
            if (!confirm)
                return;

            try
            {
                // Отправляем запрос на удаление чата
                var response = await _httpClient.DeleteAsync($"{ApiUrl}chats/{chat.ChatId}");
                if (response.IsSuccessStatusCode)
                {
                    // Удаляем чат из коллекции
                    _chats.Remove(chat); 
                    await DisplayAlert("Успех", "Чат удален", "OK");
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
        public string ChatName { get; set; } 
        public List<UserDto> Members { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
