using BackEnd.Posts;
using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class App : Application
{
    /// <inheritdoc cref="IFrontEnd"/>
    public IFrontEnd FrontEnd;

    /// <inheritdoc cref="Backend"/>
    public Backend BackEnd;

    private IDatabase _database;
    private MainPage _mainPage;

    public App()
    {
        InitializeComponent();
        MainPage = _mainPage = new MainPage();

        FrontEnd = new FrontEnd();
        BackEnd = new Backend();

        _database = new Database();

        FrontEnd.Init(BackEnd);
        BackEnd.Init(FrontEnd, _database);

        _mainPage.Init(BackEnd.Database);
    }

    /// <summary>
    /// Called when the application is opens
    /// </summary>
    protected override void OnStart()
    {
        base.OnStart();
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