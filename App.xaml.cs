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
                ChatViewModel.ClearCacheOnStartup();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка в ClearCacheOnStartup: " + ex);
                throw;
            }

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

        private async void LoadTheme()
        {
            try
            {
                // Плавное исчезновение интерфейса
                if (Current.MainPage != null)
                {
                    await Current.MainPage.FadeTo(0, 200);
                }

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

                // Принудительное обновление
                (Current.MainPage as IThemeAware)?.ApplyTheme();

                // Плавное появление интерфейса
                if (Current.MainPage != null)
                {
                    await Current.MainPage.FadeTo(1, 200);
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
            base.OnStart();
            // Используем правильное событие
            Application.Current.RequestedThemeChanged += OnAppThemeChanged;
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Application.Current.RequestedThemeChanged -= OnAppThemeChanged;
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

    internal interface IThemeAware
    {
        void ApplyTheme();
    }
}
