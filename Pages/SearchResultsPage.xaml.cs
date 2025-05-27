namespace MessengerMiniApp.Pages;
using MessengerServer.Models;
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
        _userId = userId;
        // Фильтруем: исключаем пользователя с id == _userId
        var filteredUsers = users.Where(u => u.UserId != _userId).ToList();
        _users = new ObservableCollection<User>(filteredUsers);
        searchResultsListView.ItemsSource = _users;
    }

    private async void OnUserTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is User user)
        {
            // ������� ����� ��� � ��������� �������������
            var newChat = new ChatCreationRequest
            {
                ChatName = user.Username,
                UserIds = new List<int> { _userId, user.UserId }
            };

            // ��������� ��� � ���������� � ���� ������
            var chatId = await SaveChatAndMembers(newChat);

            if (chatId != -1)
            {
                // ��������� �� �������� ����
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
        var httpClient = new HttpClient();
        var chatContent = new StringContent(JsonConvert.SerializeObject(chatRequest), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"https://noitorraa-messengerserver-c2cc.twc1.net/api/users/chats", chatContent);
        if (response.IsSuccessStatusCode)
        {
            var createdChat = JsonConvert.DeserializeObject<Chat>(await response.Content.ReadAsStringAsync());
            return createdChat.ChatId;
        }

        return -1;
    }
}

public class ChatCreationRequest
{
    public string ChatName { get; set; }
    public List<int> UserIds { get; set; }
}
