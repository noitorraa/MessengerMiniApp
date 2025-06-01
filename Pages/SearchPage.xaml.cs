using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessengerServer.Models;
using Newtonsoft.Json;

namespace MessengerMiniApp.Pages
{
    public partial class SearchPage : ContentPage
    {
        private readonly ObservableCollection<User> _searchResults = new();
        private readonly HttpClient _httpClient = new();
        private readonly string _apiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";
        private readonly int _userId;
        private CancellationTokenSource _searchCts = new();
        private bool _isSearching = false;

        public SearchPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            BindingContext = this;
            searchResultsListView.ItemsSource = _searchResults;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var query = e.NewTextValue?.Trim();

            if (_isSearching)
            {
                _searchCts.Cancel(); // Отменяем предыдущий запрос
                _searchCts = new CancellationTokenSource();
            }

            if (string.IsNullOrEmpty(query))
            {
                ClearResults();
                return;
            }

            _isSearching = true;
            ShowLoadingIndicator();

            Task.Delay(300, _searchCts.Token).ContinueWith(async (t) =>
            {
                if (t.IsCanceled) return;

                await PerformSearchAsync(query);
                HideLoadingIndicator();
                _isSearching = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task PerformSearchAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}search?login={Uri.EscapeDataString(query)}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<User>>(content);

                    UpdateResults(users.Where(u => u.UserId != _userId));
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось выполнить поиск", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка сети: {ex.Message}", "OK");
            }
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            // Логика переключения темы (будет реализована позже)
        }

        private void UpdateResults(IEnumerable<User> results)
        {
            _searchResults.Clear();
            foreach (var user in results)
            {
                _searchResults.Add(user);
            }
        }

        private void ClearResults()
        {
            _searchResults.Clear();
        }

        private void ShowLoadingIndicator()
        {
            searchActivityIndicator.IsVisible = true;
            searchActivityIndicator.IsRunning = true;
        }

        private void HideLoadingIndicator()
        {
            searchActivityIndicator.IsVisible = false;
            searchActivityIndicator.IsRunning = false;
        }

        private async void OnUserTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is User selectedUser)
            {
                var chatId = await SaveChatAndMembers(selectedUser);
                if (chatId != -1)
                {
                    await Navigation.PushAsync(new ChatPage(_userId, chatId));
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось создать чат", "OK");
                }
            }
        }

        private async Task<int> SaveChatAndMembers(User selectedUser)
        {
            try
            {
                var chatRequest = new ChatCreationRequest
                {
                    ChatName = selectedUser.Username,
                    UserIds = new List<int> { _userId, selectedUser.UserId }
                };

                var json = JsonConvert.SerializeObject(chatRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiUrl}chats", content);

                if (response.IsSuccessStatusCode)
                {
                    var createdChat = JsonConvert.DeserializeObject<Chat>(await response.Content.ReadAsStringAsync());
                    return createdChat?.ChatId ?? -1;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

public class ChatCreationRequest
{
    public string ChatName { get; set; } = null!;
    public List<int> UserIds { get; set; } = null!;
}
