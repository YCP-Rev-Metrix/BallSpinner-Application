using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.BackEnd.Database;
using System.Collections.ObjectModel;
using System.Windows.Input;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// View for the main page
/// </summary>
public partial class MainPage : ContentPage
{
    private FrontEnd _frontEnd = null!;
    private IDatabase _database = null!;

    public ObservableCollection<BallSpinnerViewModel> BallSpinners { get; } = new() { new BallSpinnerViewModel(new Simulation()) };

    /// <summary/>
    public MainPage()
    {
        InitializeComponent();

        BindingContext = this;
    }

    public void Init(FrontEnd frontEnd, IDatabase database)
    {
        _frontEnd = frontEnd;
        _database = database;
    }

    private async void OnRegisterClicked(object sender, EventArgs args)
    {
        var firstname = RegisterFirstName.Text;
        var lastname = RegisterLastName.Text;
        var username = RegisterUsername.Text;
        var password = RegisterPassword.Text;
        var email = RegisterEmail.Text;
        var phone = RegisterPhone.Text;
        var token = await _database.RegisterUser(firstname, lastname, username, password, email, phone);
        Console.WriteLine($"Auth: {token?.TokenA}");
        Console.WriteLine($"Refresh: {token?.TokenB}");
    }
    
    private async void OnLoginClicked(object sender, EventArgs args)
    {
        var username = LoginUsername.Text;
        var password = LoginPassword.Text;
        
        var token = await _database.LoginUser(username, password);
        Console.WriteLine($"Auth: {token?.TokenA}");
        Console.WriteLine($"Refresh: {token?.TokenB}");
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
        foreach (var spinner in BallSpinners)
        {
            spinner.Start();
        }
    }

    private void OnStopButtonClicked(object sender, EventArgs args)
    {
        foreach (var spinner in BallSpinners)
        {
            spinner.Stop();
        }
    }

    private void OnResetButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private async void OnAddBallSpinnerButtonClicked(object sender, EventArgs args)
    {
        var ballSpinner = await _frontEnd.AddBallSpinner();

        if(ballSpinner != null)
        {
            BallSpinners.Add(new BallSpinnerViewModel(ballSpinner));
            OnPropertyChanged(nameof(BallSpinners));
        }
    }
}