using CS341GroupProject.Model;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace CS341GroupProject;
public static class MauiProgram
{
    public static IBusinessLogic BusinessLogic = new BusinessLogic();
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiMaps().UseMauiCommunityToolkit();

        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}