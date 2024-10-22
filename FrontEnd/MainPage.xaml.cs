using BackEnd.Posts;
using System.Windows.Input;

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

    private async void OnButtonClicked(object sender, EventArgs args)
    {
        await LoginUser.LoginAsync();
        Console.WriteLine("Logged in");
    }

    private void OnNewShotButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnLoadShotButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnSaveShotButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnOptionsButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnExitButtonClicked(object sender, EventArgs args)
    {
        //Prompt if they want to save first
        Application.Current!.Quit();
    }

    private void OnResetButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnHelpButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnButtonAClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnButtonBClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnAddBallSpinnerButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }
}