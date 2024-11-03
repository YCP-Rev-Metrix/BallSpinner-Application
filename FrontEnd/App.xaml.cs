using RevMetrix.BallSpinner.BackEnd.Database;
using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class App : Application
{
    /// <inheritdoc cref="IFrontEnd"/>
    public FrontEnd FrontEnd;

    /// <inheritdoc cref="Backend"/>
    public Backend BackEnd;

    private MainPage _mainPage = null!;
    private IDatabase _database;

    public App()
    {
        InitializeComponent();
        
        MainPage = new AppShell();
        
        FrontEnd = new FrontEnd();
        BackEnd = new Backend();

        _database = new Database();

        FrontEnd.Init(BackEnd);
        BackEnd.Init(FrontEnd, _database);
        
        MainPage.Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object? sender, EventArgs e)
    {
        _mainPage = (MainPage)((AppShell)MainPage!).CurrentPage;
        _mainPage.Init(FrontEnd, BackEnd.Database);
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