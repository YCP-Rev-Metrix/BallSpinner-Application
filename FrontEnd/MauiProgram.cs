using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace RevMetrix.BallSpinner.FrontEnd;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windowsLifecycleBuilder =>
            {
                windowsLifecycleBuilder.OnWindowCreated(window =>
                {
                    window.ExtendsContentIntoTitleBar = false;
                });
            });
        });
#endif

        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<OpenGLView, OpenGLViewHandler>();
        });

        return builder.Build();
    }
}
