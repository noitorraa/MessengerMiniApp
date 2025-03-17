using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp
{
    public class SignalRService
    {
        private HubConnection _hubConnection;

        public event Action OnChatListUpdated;

        public SignalRService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://noitorraa-messengerserver-ba27.twc1.net/chatHub")
                .Build();

            _hubConnection.On("NotifyUpdateChatList", () =>
            {
                OnChatListUpdated?.Invoke();
            });
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR подключен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения SignalR: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            await _hubConnection.StopAsync();
        }
    }
}
