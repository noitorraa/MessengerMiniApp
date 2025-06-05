// Pages/ChatPage.xaml.cs
using System;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using MessengerMiniApp.ViewModels;

namespace MessengerMiniApp.Pages
{
    public partial class ChatPage : ContentPage
    {
        private readonly ChatViewModel _viewModel;

        public ChatPage(int userId, int chatId)
        {
            InitializeComponent();

            _viewModel = new ChatViewModel(userId, chatId);
            BindingContext = _viewModel;

            // Подписываемся на изменение коллекции сообщений, чтобы скроллить вниз
            _viewModel.Messages.CollectionChanged += OnMessagesCollectionChanged;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Загружаем историю и подключаем SignalR
            await _viewModel.InitializeAsync();

            // После загрузки истории скроллим к последнему сообщению
            if (_viewModel.Messages.Count > 0)
            {
                var last = _viewModel.Messages[^1];
                MessagesCollectionView.ScrollTo(last, position: ScrollToPosition.End, animate: false);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Отписываемся, чтобы не было утечки
            _viewModel.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }

        private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Если новые сообщения были добавлены
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Берём последний и скроллим с анимацией
                var last = _viewModel.Messages[^1];
                MessagesCollectionView.Dispatcher.Dispatch(() =>
                {
                    MessagesCollectionView.ScrollTo(last, position: ScrollToPosition.End, animate: true);
                });
            }
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
