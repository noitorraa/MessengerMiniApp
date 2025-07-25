using MessengerMiniApp.Pages;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace MessengerMiniApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Roboto-Regular.ttf", "Roboto");
                    fonts.AddFont("Roboto-Medium.ttf", "RobotoMedium");
                    fonts.AddFont("Lato-Regular.ttf", "Lato");
                    fonts.AddFont("Lato-Bold.ttf", "LatoBold");
                });
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddTransient<ChatPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
