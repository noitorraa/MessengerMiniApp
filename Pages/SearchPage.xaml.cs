using MessengerServer.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MessengerMiniApp.Pages
{
    public partial class SearchPage : ContentPage
    {
        private readonly ObservableCollection<User> _allUsers;
        private readonly ObservableCollection<User> _filteredUsers;
        private readonly int _userId;

        public SearchPage(List<User> users, int userId)
        {
            InitializeComponent();
            
            _userId = userId;
            _allUsers = new ObservableCollection<User>(users.Where(u => u.UserId != _userId));
            _filteredUsers = new ObservableCollection<User>(_allUsers);
            
            searchResultsListView.ItemsSource = _filteredUsers;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e) // нужно оптимизировать поиск, для того, чтобы отображались не все пользователи, например если пользователей будет миллион, чтобы поиск не пытался вывести миллион записей
        {
            string query = e.NewTextValue?.Trim().ToLower();

            if (string.IsNullOrEmpty(query))
            {
                _filteredUsers.Clear();
                foreach (var user in _allUsers)
                {
                    _filteredUsers.Add(user);
                }
                return;
            }

            _filteredUsers.Clear();

            foreach (var user in _allUsers)
            {
                if (user.Username.ToLower().Contains(query))
                {
                    _filteredUsers.Add(user);
                }
            }
        }

        private async void OnUserTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is User user)
            {
                var newChat = new ChatCreationRequest
                {
                    ChatName = user.Username,
                    UserIds = new List<int> { _userId, user.UserId }
                };

                var chatId = await SaveChatAndMembers(newChat);
                
                if (chatId != -1)
                {
                    await Navigation.PushAsync(new ChatPage(_userId, chatId));
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось перейти в чат", "OK");
                }
            }
        }

        private async Task<int> SaveChatAndMembers(ChatCreationRequest chatRequest)
        {
            try
            {
                var httpClient = new HttpClient();
                var json = JsonConvert.SerializeObject(chatRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(
                    "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/chats ",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    var createdChat = JsonConvert.DeserializeObject<Chat>(
                        await response.Content.ReadAsStringAsync()
                    );
                    return createdChat?.ChatId ?? -1;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }
    }
}

public class ChatCreationRequest
{
    public string ChatName { get; set; } = null!;
    public List<int> UserIds { get; set; } = null!;
}
