using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;

namespace MessengerMiniApp.Pages
{
    public partial class ChatPage : ContentPage
    {
        private HubConnection _hubConnection;
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://noitorraa-messengerserver-7295.twc1.net/api/users/";
        private readonly int _userId;
        private readonly int _chatId;

        //private readonly IAudioManager _audioManager;
        //private AudioPlayer _audioRecorder;
        //private IAudioPlayer _audioPlayer;
        //private string _tempFilePath;

        private ObservableCollection<string> _messages;

        public ChatPage(int userId, int chatId)
        {
            InitializeComponent();
            //_audioManager = audioManager;
            _userId = userId;
            _chatId = chatId;
            _messages = new ObservableCollection<string>();
            MessagesCollectionView.ItemsSource = _messages;
            LoadMessages();
            _ = ConnectToSignalR(); // Используем дискорд для асинхронного вызова
        }

        private async void LoadMessages()
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}chats/{_chatId}/messages");
            if (response.IsSuccessStatusCode)
            {
                var messages = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());
                foreach (var message in messages)
                {
                    _messages.Add(message);
                }
            }
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageEntry.Text))
            {
                await _hubConnection.InvokeAsync("SendMessage", _userId, MessageEntry.Text, _chatId);
                MessageEntry.Text = string.Empty;
            }
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

            // Подписка на получение сообщений
            _hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _messages.Add(message); // Добавление строки в коллекцию
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinChat", _chatId); // Вход в группу
                Console.WriteLine("Успешное подключение к чату");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }

        private void OnVoiceRecordPressed(object sender, EventArgs e)
        {

        }

        private void OnVoiceRecordReleased(object sender, EventArgs e)
        {

        }

        //private async void OnVoiceRecordPressed(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var status = await Permissions.RequestAsync<Permissions.Microphone>();
        //        if (status != PermissionStatus.Granted)
        //        {
        //            await DisplayAlert("Ошибка", "Требуется доступ к микрофону", "OK");
        //            return;
        //        }

        //        // Создаем временный файл
        //        _tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.wav");

        //        // Инициализируем рекордер с указанием пути
        //        _audioRecorder = _audioManager.CreateRecorder(
        //            new AudioRecorderOptions
        //            {
        //                FilePath = _tempFilePath // Указываем путь для сохранения
        //            }
        //        );

        //        // Начинаем запись
        //        _audioRecorder.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Ошибка", $"Не удалось начать запись: {ex.Message}", "OK");
        //    }
        //}

        //private async void OnVoiceRecordReleased(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (_audioRecorder == null)
        //            return;

        //        // Остановка записи
        //        var audioStream = await _audioRecorder.Stop();

        //        if (audioStream == null || audioStream.Length == 0)
        //        {
        //            // Обработка ситуации с пустым потоком
        //            return;
        //        }

        //        // Сброс позиции потока на начало
        //        if (audioStream.CanSeek)
        //            audioStream.Seek(0, SeekOrigin.Begin);

        //        // Сохранение аудио во временный файл
        //        var tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.wav");

        //        using (var fileStream = File.Create(tempFilePath))
        //        {
        //            await audioStream.CopyToAsync(fileStream);
        //        }

        //        // Закрытие исходного потока (если требуется)
        //        await audioStream.DisposeAsync();

        //        // Отправка аудио-сообщения
        //        await SendAudioMessage(tempFilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Обработка исключений (логирование, уведомление пользователя)
        //        Console.WriteLine($"Ошибка обработки аудио: {ex}");
        //    }
        //}

        //private async Task SendAudioMessage(string filePath)
        //{
        //    // Отправка аудио через API
        //    try
        //    {
        //        using var fileStream = File.OpenRead(filePath);
        //        var content = new MultipartFormDataContent
        //    {
        //        { new StreamContent(fileStream), "file", "audio.wav" },
        //        { new StringContent(_chatId.ToString()), "chatId" },
        //        { new StringContent(_userId.ToString()), "senderId" }
        //    };

        //        var response = await _httpClient.PostAsync("api/media/upload", content);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Обновление списка сообщений
        //            LoadMessages();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", ex.Message, "OK");
        //    }
        //}

        //private async void PlayAudio(string fileUrl)
        //{
        //    // Воспроизведение аудио
        //    _audioPlayer = _audioManager.CreatePlayer(await _httpClient.GetStreamAsync(fileUrl));
        //    _audioPlayer.Play();
        //}
    }
}