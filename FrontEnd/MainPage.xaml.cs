using BackEnd.Posts;

namespace RevMetrix.BallSpinner.FrontEnd;


/// <summary>
/// View for the main page
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary/>
    public MainPage()
    {
        InitializeComponent();
    }
    async void OnButtonClicked(object sender, EventArgs args)
    {
        Database database = new Database();
        database.LoginUser();
        //await LoginUser.Main();
        Console.WriteLine("Logged in");
    }

}