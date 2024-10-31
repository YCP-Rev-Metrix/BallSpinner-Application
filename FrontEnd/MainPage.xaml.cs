using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Database;
using System.Windows.Input;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// View for the main page
/// </summary>
public partial class MainPage : ContentPage
{
    private FrontEnd _frontEnd = null!;
    private IDatabase _database = null!;

    //public MainPage MainPage => Content;

    /// <summary/>
    public MainPage()
    {
        InitializeComponent();
    }

    public void Init(FrontEnd frontEnd, IDatabase database)
    {
        _frontEnd = frontEnd;
        _database = database;
    }

    private async void OnButtonClicked(object sender, EventArgs args)
    {
        await _database.LoginUser();
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

    private void OnResetViewButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnHelpButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.Help();
    }

    private void OnStartButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnStopButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnResetButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnAddBallSpinnerButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }
}