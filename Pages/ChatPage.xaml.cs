using System;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using MessengerMiniApp.ViewModels;
using MessengerServer.Model;
using System.Diagnostics;

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
            // Если загружена история (addToTop), пытаемся восстановить позицию
            if (e.Action == NotifyCollectionChangedAction.Add && _viewModel._firstVisibleMessageBeforeLoad != null)
            {
                var target = _viewModel._firstVisibleMessageBeforeLoad;
                int idx = _viewModel.Messages.IndexOf(target);
                if (idx >= 0)
                {
                    // Ждём обновления UI, а потом скроллим
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(100);
                        try
                        {
                            MessagesCollectionView.ScrollTo(target, position: ScrollToPosition.Start, animate: false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"ScrollTo failed: {ex}");
                        }
                        _viewModel._firstVisibleMessageBeforeLoad = null;
                    });
                }
                else
                {
                    // Элемент не найден — сбросим, чтобы не пытаться снова
                    _viewModel._firstVisibleMessageBeforeLoad = null;
                }
            }
            // Для новых сообщений в конец
            else if (e.Action == NotifyCollectionChangedAction.Add
                  || e.Action == NotifyCollectionChangedAction.Replace)
            {
                var last = _viewModel.Messages.LastOrDefault();
                if (last != null)
                {
                    try
                    {
                        MessagesCollectionView.ScrollTo(last, position: ScrollToPosition.End, animate: true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"ScrollTo last failed: {ex}");
                    }
                }
            }
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();

            // Дожидаемся, когда UICollectionView обновится
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                if (_viewModel.Messages.Count > 0)
                {
                    var last = _viewModel.Messages[^1];
                    try
                    {
                        MessagesCollectionView.ScrollTo(last, position: ScrollToPosition.End, animate: false);
                    }
                    catch { /* игнорируем */ }
                }
            });
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
