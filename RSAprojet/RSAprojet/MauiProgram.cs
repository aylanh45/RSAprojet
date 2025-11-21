using Microsoft.Extensions.Logging;
using RSAprojet.Services;
using RSAprojet.Platforms.Android.Services;

namespace RSAprojet
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if ANDROID
            // 👉 Enregistrement NFC Android dans DI
            builder.Services.AddSingleton<INfcService, NfcService_Android>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}