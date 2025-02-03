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
            Console.WriteLine("������� ������ ��� ���������� ������ �����");
            await LoadChats();
        }

        protected override void OnDisappearing()
        {
            // ������� ��� ����� �� ��������
            App.signalRService.OnChatListUpdated -= OnChatListUpdated;
            base.OnDisappearing();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ConnectToSignalR();
            await LoadChats(); // ��������� ������ ����� ��� ������ ��������� ��������
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
                            // ��������� �������� �� �������
                            chat.ChatName = GetChatName(chat.Members, _userId);
                            _chats.Add(chat);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", ex.Message, "OK");
            }
        }

        private string GetChatName(List<UserDto> members, int currentUserId)
        {
            if (members.Count == 1) return "������ ��� (������)";

            // ��� ������ �����: ��� �����������
            if (members.Count == 2)
            {
                return members.First(u => u.UserId != currentUserId).Username;
            }

            // ��� ��������� �����: ������ ���� ����������
            return string.Join(", ", members.Select(u => u.Username));
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

            _hubConnection.On("NotifyUpdateChatList", async () =>
            {
                Console.WriteLine("������� ������ ��� ���������� ������ �����");
                await LoadChats(); // ��������� ����������� ������ �����
            });

            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine($"��������� �����������: {_hubConnection.State}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �����������: {ex.Message}");
            }
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            var searchQuery = searchBar.Text;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await DisplayAlert("������", "������� �����", "OK");
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
                        await DisplayAlert("����������", "������������ �� �������", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("������", "�� ������� ��������� �����", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", ex.Message, "OK");
            }
        }

        private async void OnChatTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is ChatDto chat)
            {
                await Navigation.PushAsync(new ChatPage(_userId, chat.ChatId));
            }
        }

        private async void OnDeleteChat(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem != null && menuItem.CommandParameter is int chatId)
            {
                var result = await DisplayAlert("�������� ����", "�� ������������� ������ ������� ���� ���?", "��", "���");
                if (result)
                {
                    await DeleteChat(chatId);
                }
            }
        }

        private async Task DeleteChat(int chatId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiUrl}chats/{chatId}");
                if (response.IsSuccessStatusCode)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var chatToRemove = _chats.FirstOrDefault(c => c.ChatId == chatId);
                        if (chatToRemove != null)
                        {
                            _chats.Remove(chatToRemove);
                        }
                    });
                }
                else
                {
                    await DisplayAlert("������", "�� ������� ������� ���", "��");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", ex.Message, "��");
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
