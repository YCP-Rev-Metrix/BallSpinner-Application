using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class App : Application
{
    /// <inheritdoc cref="IFrontEnd"/>
    public IFrontEnd FrontEnd;

    /// <inheritdoc cref="Backend"/>
    public Backend BackEnd;

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        FrontEnd = new FrontEnd();
        BackEnd = new Backend();

        FrontEnd.Init(BackEnd);
        BackEnd.Init(FrontEnd);
    }

    /// <summary>
    /// Called when the application is opens
    /// </summary>
    protected override void OnStart()
    {
        base.OnStart();

        //FrontEnd.Alert("Game over, man");
    }

    /// <summary>
    /// Called when the application is closing
    /// </summary>
    protected override void CleanUp()
    {
        FrontEnd.Dispose();
        BackEnd.Dispose();
    }
}