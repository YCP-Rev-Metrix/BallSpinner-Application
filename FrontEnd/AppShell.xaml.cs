namespace RevMetrix.BallSpinner.FrontEnd;

public partial class AppShell : Shell
{

    public AppShell()
    {
        InitializeComponent();
    }

    public void SetContent(MainPage page)
    {
        Content = new MainPage();
    }
}