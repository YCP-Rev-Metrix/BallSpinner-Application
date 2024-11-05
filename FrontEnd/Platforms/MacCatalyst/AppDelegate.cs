using Foundation;
using UIKit;

namespace RevMetrix.BallSpinner.FrontEnd;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    //public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    //{
        // Override point for customization after application launch.
        //return true;
    //}
    public override void OnActivated(UIApplication uiApplication)
    {
        // Restart any tasks that were paused (or not yet started) while the application was inactive.
    }

    public override void OnResignActivation(UIApplication uiApplication)
    {
        // This method is called when the application is about to move from active to inactive state.
    }


    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

}
