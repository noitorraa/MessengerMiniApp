using MessengerMiniApp.Pages;

namespace MessengerMiniApp
{
    public partial class App : Application
    {
        public static SignalRService signalRService { get; private set; }
        public App()
        {
            InitializeComponent();
            signalRService = new SignalRService();
            _ = signalRService.StartAsync();
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
