using MessengerMiniApp.Pages;
using MessengerMiniApp.ViewModels;
using System.Diagnostics;
using System.Reflection;

namespace MessengerMiniApp
{
    /// <summary>
    /// Main application class that initializes the MAUI application
    /// </summary>
    public partial class App : Application
    {
        public static event Action ThemeChanged;
        /// <summary>
        /// Initializes a new instance of the App class
        /// </summary>
        public App()
        {
            InitializeComponent();
            try
            {
                LoadTheme();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка в LoadTheme: " + ex);
                throw;
            }
        }

        private void LoadTheme()
        {
            try
            {
                ICollection<ResourceDictionary> mergedDictionaries = Current.Resources.MergedDictionaries;
                if (mergedDictionaries != null)
                {
                    mergedDictionaries.Clear();
                    ResourceDictionary theme = Application.Current.RequestedTheme switch
                    {
                        AppTheme.Dark => new DarkThemeResources(),
                        _ => new LightThemeResources()
                    };
                    mergedDictionaries.Add(theme);
                }

                // Пробрасываем ApplyTheme до текущей страницы
                if (Current.MainPage is NavigationPage navPage &&
                    navPage.CurrentPage is IThemeAware currentPage)
                {
                    currentPage.ApplyTheme();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error changing theme: {ex}");
            }
        }

        private void OnAppThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            LoadTheme();
            ThemeChanged?.Invoke(); // Уведомляем все страницы об изменении темы
        }

        protected override void OnStart()
        {
            ChatViewModel.ClearOldCache();
            // Используем правильное событие
            Application.Current.RequestedThemeChanged += OnAppThemeChanged;
            base.OnStart();
        }

        protected override void OnSleep()
        {
            var currentPage = Current.MainPage;
            if (currentPage is NavigationPage navPage && navPage.CurrentPage is ChatPage chatPage)
            {
                var vm = chatPage.BindingContext as ChatViewModel;
                vm?.SaveMessagesToCache();
            }
            Application.Current.RequestedThemeChanged -= OnAppThemeChanged;
            base.OnSleep();
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
            var currentPage = Current.MainPage;
            if (currentPage is NavigationPage navPage && navPage.CurrentPage is ChatPage chatPage)
            {
                var vm = chatPage.BindingContext as ChatViewModel;
                vm?.LoadMessagesFromCache();
            }
            base.OnResume();
        }
    }

    internal interface IThemeAware
    {
        void ApplyTheme();
    }
}
