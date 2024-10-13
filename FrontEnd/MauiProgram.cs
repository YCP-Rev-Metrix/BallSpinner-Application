using Microsoft.Extensions.Logging;

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

		return builder.Build();
	}
}
