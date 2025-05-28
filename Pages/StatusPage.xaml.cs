using System.Collections.ObjectModel;
using System.Linq;
using MessengerServer.Models;

namespace MessengerMiniApp.Pages
{
    public partial class StatusPage : ContentPage
    {
        private readonly int _userId;
        private ObservableCollection<StatusItemDto> _statuses;

        public StatusPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _statuses = new ObservableCollection<StatusItemDto>();
            statusListView.ItemsSource = _statuses;
            
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            // Implement navigation back logic
            _ = Navigation.PopAsync();
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            // Implement theme toggle logic
        }

        private async void OnStatusTabClicked(object sender, EventArgs e)
        {
            // Already on status page, no action needed
        }

        private async void OnChatsTabClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }

    public class StatusItemDto
    {
        public string StatusName { get; set; }
        public string StatusText { get; set; }
        public string Avatar { get; set; }
    }
}
