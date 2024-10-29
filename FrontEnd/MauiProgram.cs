using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Windowing;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// Earliest point in the application lifetime
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Tells MAUI what view the core of the application should use.
    /// This is equivalent to the program constructor.
    /// </summary>
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
                bool rootWindow = true;
                windowsLifecycleBuilder.OnWindowCreated(window =>
                {
                    window.ExtendsContentIntoTitleBar = false;
                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                    if(appWindow.Presenter is OverlappedPresenter overlappedPresenter)
                    {
                        if (rootWindow)
                        {
                            //main page should be maximized
                            overlappedPresenter.Maximize();
                            rootWindow = false;
                        }
                        else
                        {
                            //popups should always be above the main page
                            overlappedPresenter.IsAlwaysOnTop = true;
                        }

                        //pages should always start centered to the window
                        DisplayArea displayArea = DisplayArea.GetFromWindowId(id, DisplayAreaFallback.Nearest);
                        if (displayArea is not null)
                        {
                            var centeredPosition = appWindow.Position;
                            centeredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                            centeredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                            appWindow.Move(centeredPosition);
                        }
                    }
                });
            });
        });
#endif

        return builder.Build();
    }
}
