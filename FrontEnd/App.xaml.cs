namespace RevMetrix.BallSpinner.FrontEnd;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override void CleanUp()
    {
        //Dispose
    }
}