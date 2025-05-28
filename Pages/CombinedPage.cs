using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using MessengerServer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerMiniApp.Pages
{
    public partial class CombinedPage : ContentPage
    {
        // Общие поля
        private readonly int _userId;
        
        // Поля для вкладки чатов
        private HubConnection? _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";
        private ObservableCollection<ChatDto> _chats;
        
        // Текущая активная вкладка
        private enum ActiveTab { Chats, Profile }
        private ActiveTab _currentTab = ActiveTab.Chats;

        public CombinedPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            
            // Инициализация коллекции чатов
            _chats = new ObservableCollection<ChatDto>();
            ChatListView.ItemsSource = _chats;
            
            // Загружаем данные чатов по умолчанию
            _ = LoadChats();
            
            // Установка начального состояния интерфейса
            UpdateUI();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Подключаемся к SignalR для чатов
            await ConnectToSignalR();
            
            // Загружаем данные в зависимости от текущей вкладки
            if (_currentTab == ActiveTab.Chats)
            {
                await LoadChats();
            }
            else
            {
                await LoadUserProfile();
            }
        }

        #region Общие методы

        private void UpdateUI()
        {
            // Обновляем состояние UI в зависимости от выбранной вкладки
            if (_currentTab == ActiveTab.Chats)
            {
                // Показываем чаты
                ChatListView.IsVisible = true;
                ProfileView.IsVisible = false;
                SearchButton.IsVisible = true;
                
                // Обновляем стили кнопок
                ChatsTabButton.TextColor = Colors.White;
                ChatsTabButton.BackgroundColor = (Color)Application.Current.Resources["Secondary"];
                
                StatusTabButton.TextColor = Colors.Black;
                StatusTabButton.BackgroundColor = Colors.White;
            }
            else // ActiveTab.Profile
            {
                // Показываем профиль пользователя
                ChatListView.IsVisible = false;
                ProfileView.IsVisible = true;
                SearchButton.IsVisible = false; // Скрываем кнопку поиска в режиме профиля
                
                // Обновляем стили кнопок
                StatusTabButton.TextColor = Colors.White;
                StatusTabButton.BackgroundColor = (Color)Application.Current.Resources["Secondary"];
                
                ChatsTabButton.TextColor = Colors.Black;
                ChatsTabButton.BackgroundColor = Colors.White;
            }
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            // Логика переключения темы (будет реализована позже)
        }

        #endregion

        #region Методы для вкладки чатов

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

        private async void OnSearchButtonClicked(object sender, EventArgs e)
        {
            var image = sender as ImageButton; // Изменил тип на ImageButton, поскольку в XAML используется ImageButton
            // Анимация уменьшения при нажатии
            await image.ScaleTo(0.95, 50, Easing.SinOut);
            await image.ScaleTo(1, 50, Easing.SinIn);
            await Navigation.PushAsync(new SearchPage(_userId));
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

            ChatListView.SelectedItem = null;
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

        #endregion

        #region Методы для вкладки профиля

        private async Task LoadUserProfile()
        {
            try
            {
                // Получаем информацию о пользователе с сервера
                var response = await _httpClient.GetAsync($"{ApiUrl}{_userId}");
                if (response.IsSuccessStatusCode)
                {
                    var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                    if (user != null)
                    {
                        // Заполняем поля профиля
                        UsernameEntry.Text = user.Username;
                        
                        // Если у пользователя есть аватар, отображаем его
                        //if (!string.IsNullOrEmpty(user.Avatar))
                        //{
                        //    UserAvatar.Source = user.Avatar;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить профиль: {ex.Message}", "OK");
            }
        }

        private async void OnChangeAvatarClicked(object sender, EventArgs e)
        {
            try
            {
                // Запрашиваем фото из галереи
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    // Отображаем выбранное фото
                    UserAvatar.Source = ImageSource.FromFile(photo.FullPath);
                    
                    // Логика для сохранения аватара на сервере будет добавлена позже
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось выбрать фото: {ex.Message}", "OK");
            }
        }

        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            try
            {
                // Проверяем заполнение полей
                if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
                {
                    await DisplayAlert("Ошибка", "Пожалуйста, введите имя пользователя", "OK");
                    return;
                }

                // Создаем объект с данными пользователя для отправки на сервер
                var userData = new
                {
                    UserId = _userId,
                    Username = UsernameEntry.Text,
                    // Логика для отправки аватара будет добавлена позже
                };

                // Отправляем данные на сервер
                var content = new StringContent(
                    JsonConvert.SerializeObject(userData),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PutAsync($"{ApiUrl}{_userId}", content);
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успешно", "Профиль обновлен", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось обновить профиль", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось сохранить профиль: {ex.Message}", "OK");
            }
        }

        #endregion

        #region Обработчики переключения вкладок

        private async void OnStatusTabClicked(object sender, EventArgs e)
        {
            if (_currentTab != ActiveTab.Profile)
            {
                _currentTab = ActiveTab.Profile;
                await LoadUserProfile(); // Загружаем данные профиля
                UpdateUI();            // Обновляем интерфейс
            }
        }

        private async void OnChatsTabClicked(object sender, EventArgs e)
        {
            if (_currentTab != ActiveTab.Chats)
            {
                _currentTab = ActiveTab.Chats;
                await LoadChats(); // Загружаем данные для вкладки чатов
                UpdateUI();        // Обновляем интерфейс
            }
        }

        #endregion
    }

    // Модель для чатов (из ChatListPage)
    public class ChatDto
    {
        public int ChatId { get; set; }
        public string? ChatName { get; set; }
        public List<UserDto>? Members { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LastMessage { get; set; }
        public string? Avatar { get; set; } = "usericon.png";
    }

    // Модель для пользователей (из ChatListPage)
    public class UserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
    }
}