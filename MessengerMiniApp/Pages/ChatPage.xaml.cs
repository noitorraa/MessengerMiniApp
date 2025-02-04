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
            _ = ConnectToSignalR(); // ���������� ������� ��� ������������ ������
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

            // �������� �� ��������� ���������
            _hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _messages.Add(message); // ���������� ������ � ���������
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinChat", _chatId); // ���� � ������
                Console.WriteLine("�������� ����������� � ����");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �����������: {ex.Message}");
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
        //            await DisplayAlert("������", "��������� ������ � ���������", "OK");
        //            return;
        //        }

        //        // ������� ��������� ����
        //        _tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.wav");

        //        // �������������� �������� � ��������� ����
        //        _audioRecorder = _audioManager.CreateRecorder(
        //            new AudioRecorderOptions
        //            {
        //                FilePath = _tempFilePath // ��������� ���� ��� ����������
        //            }
        //        );

        //        // �������� ������
        //        _audioRecorder.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("������", $"�� ������� ������ ������: {ex.Message}", "OK");
        //    }
        //}

        //private async void OnVoiceRecordReleased(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (_audioRecorder == null)
        //            return;

        //        // ��������� ������
        //        var audioStream = await _audioRecorder.Stop();

        //        if (audioStream == null || audioStream.Length == 0)
        //        {
        //            // ��������� �������� � ������ �������
        //            return;
        //        }

        //        // ����� ������� ������ �� ������
        //        if (audioStream.CanSeek)
        //            audioStream.Seek(0, SeekOrigin.Begin);

        //        // ���������� ����� �� ��������� ����
        //        var tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.wav");

        //        using (var fileStream = File.Create(tempFilePath))
        //        {
        //            await audioStream.CopyToAsync(fileStream);
        //        }

        //        // �������� ��������� ������ (���� ���������)
        //        await audioStream.DisposeAsync();

        //        // �������� �����-���������
        //        await SendAudioMessage(tempFilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        // ��������� ���������� (�����������, ����������� ������������)
        //        Console.WriteLine($"������ ��������� �����: {ex}");
        //    }
        //}

        //private async Task SendAudioMessage(string filePath)
        //{
        //    // �������� ����� ����� API
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
        //            // ���������� ������ ���������
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
        //    // ��������������� �����
        //    _audioPlayer = _audioManager.CreatePlayer(await _httpClient.GetStreamAsync(fileUrl));
        //    _audioPlayer.Play();
        //}
    }
}