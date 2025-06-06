using MessengerMiniApp.Pages;
using MessengerMiniApp.ViewModels;

namespace MessengerMiniApp
{
    /// <summary>
    /// Main application class that initializes the MAUI application
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class
        /// </summary>
        public App()
        {
            InitializeComponent();
            ChatViewModel.ClearCacheOnStartup();
        }

        /// <summary>
        /// Creates the main window for the application
        /// </summary>
        /// <param name="activationState">Optional activation state</param>
        /// <returns>A new Window instance with the application's main page</returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Create and return a new window with the login page as the root
            return new Window(new NavigationPage(new LoginPage()));
        }

        protected override void OnResume()
        {
            // Восстанавливаем соединение при возврате в приложение
            var currentPage = Current.MainPage;
            if (currentPage is Shell shell && shell.CurrentPage?.BindingContext is ChatViewModel vm)
            {
                vm.ReconnectAsync();
            }
            base.OnResume();
        }
    }
}
