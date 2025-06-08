using System;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using MessengerMiniApp.ViewModels;

namespace MessengerMiniApp.Pages
{
    public partial class ChatPage : ContentPage
    {
        private readonly ChatViewModel _viewModel;

        public ChatPage(int userId, int chatId, string peerUsername)
        {
            InitializeComponent();

            _viewModel = new ChatViewModel(userId, chatId, peerUsername);
            BindingContext = _viewModel;

            // Подписываемся на изменение коллекции сообщений, чтобы скроллить вниз
            _viewModel.Messages.CollectionChanged += OnMessagesCollectionChanged;
        }

        private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                var last = _viewModel.Messages.LastOrDefault();
                if (last != null)
                {
                    MessagesCollectionView.ScrollTo(last, position: ScrollToPosition.End, animate: true);
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Загружаем историю и подключаем SignalR
            await _viewModel.InitializeAsync();
            if (_viewModel != null)
            {
                await _viewModel.ReconnectAsync();
                if (_viewModel.Messages.Count > 0)
                {
                    var last = _viewModel.Messages[^1];
                    MessagesCollectionView.ScrollTo(last, position: ScrollToPosition.End, animate: false);
                }
            }
        }



        protected override void OnDisappearing() // при переходе в проводник при прикреплении файла вызывается этот метод и приложение закрывается, может стоит просто как в телеграмме не переход, а доступ к галлереее сделать, где как раз таки можно также и файлы отправить
        {
            base.OnDisappearing();
            // Отписываемся, чтобы не было утечки
            _viewModel.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Возвращаемся на предыдущую страницу
            Navigation.PopAsync();
        }

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            // При нажатии Enter в Entry — выполняем команду SendMessage
            if (_viewModel.SendMessageCommand.CanExecute(null))
            {
                _viewModel.SendMessageCommand.Execute(null);
            }
        }
    }
}
