using BackEnd.Posts;

namespace RevMetrix.BallSpinner.FrontEnd;

using BackEnd;
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
        await LoginUser.Main();
        Console.WriteLine("Logged in");
    }

}