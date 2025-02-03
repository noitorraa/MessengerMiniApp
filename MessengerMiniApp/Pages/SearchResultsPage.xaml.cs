namespace MessengerMiniApp.Pages;
using MessengerServer.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;

public partial class SearchResultsPage : ContentPage
{

    private readonly ObservableCollection<User> _users;
    private readonly int _userId;

    public SearchResultsPage(List<User> users, int userId)
    {
        InitializeComponent();
        _users = new ObservableCollection<User>(users);
        _userId = userId;
        searchResultsListView.ItemsSource = _users;
    }

    private async void OnUserTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is User user)
        {
            // Создаем новый чат с выбранным пользователем
            var newChat = new ChatCreationRequest
            {
                ChatName = user.Username,
                UserIds = new List<int> { _userId, user.UserId }
            };

            // Сохраняем чат и участников в базе данных
            var chatId = await SaveChatAndMembers(newChat);

            if (chatId != -1)
            {
                // Переходим на страницу чата
                await Navigation.PushAsync(new ChatPage(_userId, chatId));
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось создать чат", "OK");
            }
        }
    }

    private async Task<int> SaveChatAndMembers(ChatCreationRequest chatRequest)
    {
        var httpClient = new HttpClient();
        var chatContent = new StringContent(JsonConvert.SerializeObject(chatRequest), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"https://noitorraa-messengerserver-7295.twc1.net/api/users/chats", chatContent);
        if (response.IsSuccessStatusCode)
        {
            var createdChat = JsonConvert.DeserializeObject<Chat>(await response.Content.ReadAsStringAsync());
            return createdChat.ChatId;
        }

        return -1; // В случае ошибки
    }
}

public class ChatCreationRequest
{
    public string ChatName { get; set; }
    public List<int> UserIds { get; set; }
}
