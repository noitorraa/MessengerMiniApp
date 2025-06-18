using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using MessengerMiniApp.DTOs;
using MessengerServer.Model;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Xaml;

namespace MessengerMiniApp.Pages
{
    public partial class CombinedPage : ContentPage, IThemeAware
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

        private async void OnThemeChanged()
        {
            await this.FadeTo(0, 200);

            // Создаем новую страницу с нулевой прозрачностью
            var newPage = new CombinedPage(_userId) { Opacity = 0 };

            await Navigation.PushAsync(newPage);
            Navigation.RemovePage(this);

            // Анимируем появление НОВОЙ страницы
            await newPage.FadeTo(1, 200);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.ThemeChanged -= OnThemeChanged; // Отписываемся
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.ThemeChanged += OnThemeChanged;
            ThemeToggle.IsToggled = Application.Current.UserAppTheme == AppTheme.Dark;
            
            // Подключаемся к SignalR для чатов
            await ConnectToSignalR();
            
            // Загружаем данные в зависимости от текущей вкладки
            if (_currentTab == ActiveTab.Chats)
            {
                await LoadChats();
            }
            else
            {
                Console.WriteLine("Загрузка профиля");
                await LoadUserProfile();
            }
        }

        #region Общие методы

        private void UpdateUI()
        {
            ChatListView.IsVisible = _currentTab == ActiveTab.Chats;
            ProfileView.IsVisible = _currentTab == ActiveTab.Profile;
            SearchButton.IsVisible = _currentTab == ActiveTab.Chats;

            if (_currentTab == ActiveTab.Chats)
            {
                // Используем динамические ресурсы
                ChatsTabButton.SetDynamicResource(Button.TextColorProperty, "ActiveTabTextColor");
                ChatsTabButton.SetDynamicResource(Button.BackgroundColorProperty, "ActiveTabBackgroundColor");

                StatusTabButton.SetDynamicResource(Button.TextColorProperty, "InactiveTabTextColor");
                StatusTabButton.SetDynamicResource(Button.BackgroundColorProperty, "InactiveTabBackgroundColor");
            }
            else
            {
                StatusTabButton.SetDynamicResource(Button.TextColorProperty, "ActiveTabTextColor");
                StatusTabButton.SetDynamicResource(Button.BackgroundColorProperty, "ActiveTabBackgroundColor");

                ChatsTabButton.SetDynamicResource(Button.TextColorProperty, "InactiveTabTextColor");
                ChatsTabButton.SetDynamicResource(Button.BackgroundColorProperty, "InactiveTabBackgroundColor");
            }
        }

        public void ApplyTheme()
        {
            if (this.IsVisible) // Обновляем только видимые страницы
            {
                UpdateUI();
            }
        }


        private void OnBackClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
            Preferences.Set("AppTheme", e.Value ? "Dark" : "Light");
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
                    await _hubConnection.InvokeAsync("RegisterUser", _userId); // Такого роута нет на сервере, потом нужно будет поменять
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
                await Navigation.PushAsync(new ChatPage(_userId, selectedChat.ChatId, selectedChat.ChatName));
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
                // Проверяем наличие аватара в локальном хранилище
                var localAvatarPath = Path.Combine(FileSystem.CacheDirectory, "MessengerFiles", $"avatar_{_userId}.jpg");
                if (System.IO.File.Exists(localAvatarPath))
                {
                    // Если аватар существует локально, загружаем его
                    using var fileStream = new FileStream(localAvatarPath, FileMode.Open, FileAccess.Read);
                    var memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);
                    UserAvatar.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                }
                else
                {
                    // Если аватара нет локально, запрашиваем его с сервера
                    var response = await _httpClient.GetAsync($"{ApiUrl}profile/{_userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                        if (user != null && user.Avatar != null)
                        {
                            // Отображаем аватар, полученный с сервера
                            UserAvatar.Source = ImageSource.FromStream(() => new MemoryStream(user.Avatar));

                            // Сохраняем аватар локально
                            using var fileStream = new FileStream(localAvatarPath, FileMode.Create, FileAccess.Write);
                            await fileStream.WriteAsync(user.Avatar, 0, user.Avatar.Length);
                        }
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
                    // Проверяем расширение файла
                    var fileExtension = Path.GetExtension(photo.FullPath).ToLowerInvariant();
                    if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(fileExtension))
                    {
                        await DisplayAlert("Ошибка", "Недопустимый формат файла. Пожалуйста, выберите изображение.", "OK");
                        return;
                    }

                    // Отображаем выбранное фото
                    UserAvatar.Source = ImageSource.FromFile(photo.FullPath);

                    // Сохраняем аватар локально
                    var localPath = Path.Combine(FileSystem.CacheDirectory, "MessengerFiles", $"avatar_{_userId}{fileExtension}");
                    using var fileStream = System.IO.File.Create(localPath);
                    using var stream = await photo.OpenReadAsync();
                    await stream.CopyToAsync(fileStream);

                    // Отправляем аватар на сервер
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var avatarBytes = memoryStream.ToArray();

                    var changeAvatarRequest = new ChangeAvatarRequest
                    {
                        UserId = _userId,
                        NewAvatar = avatarBytes
                    };

                    var content = new StringContent(
                        JsonConvert.SerializeObject(changeAvatarRequest),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PutAsync($"{ApiUrl}change-avatar", content);
                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Успешно", "Аватар обновлен", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось обновить аватар", "OK");
                    }
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
                var changeLoginRequest = new ChangeLoginRequest
                {
                    UserId = _userId,
                    NewLogin = UsernameEntry.Text
                };

                // Отправляем данные на сервер
                var content = new StringContent(
                    JsonConvert.SerializeObject(changeLoginRequest),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PutAsync($"{ApiUrl}change-login", content);
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успешно", "Логин обновлен", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось обновить логин", "OK");
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
        public byte[]? Avatar { get; set; }
    }

    // Модель для пользователей (из ChatListPage)
    public class UserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
    }
}
