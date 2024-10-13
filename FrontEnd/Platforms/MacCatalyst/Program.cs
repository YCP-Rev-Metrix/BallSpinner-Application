using ObjCRuntime;
using UIKit;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// Entry point for MAC OSX
/// </summary>
public class Program
{
    /// <summary>
    /// This is the main entry point of the application for MAC OSX only!
    /// </summary>
    public static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}