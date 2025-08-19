using Lead.Mobile.Interfaces;
using Lead.Mobile.Services;
using Microsoft.Extensions.Logging;

namespace Lead.Mobile;

public static class MauiProgram
{
    // Expose the built service provider for DI
    public static IServiceProvider Services { get; private set; } = default!;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // ----------------------------
        // Register Services / ViewModels
        // ----------------------------
        builder.Services.AddSingleton<IHttpService, HttpService>();  // Singleton for API calls
        //builder.Services.AddTransient<LoginViewModel>();              // New instance per page
        //builder.Services.AddTransient<MainPage>();                    // Optional, if you want DI for MainPage

        var app = builder.Build();

        // Save ServiceProvider globally
        Services = app.Services;

        return app;
    }
}
