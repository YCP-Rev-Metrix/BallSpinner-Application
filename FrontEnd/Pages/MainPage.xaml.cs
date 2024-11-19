using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;
using RevMetrix.BallSpinner.BackEnd.Database;
using System.Collections.ObjectModel;
using System.Windows;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// View for the main page
/// </summary>
public partial class MainPage : ContentPage
{
    private FrontEnd _frontEnd = null!;
    private IDatabase _database = null!;

    public ObservableCollection<BallSpinnerViewModel> BallSpinners { get; } = new();

    public bool NotLoggedIn => !LoggedIn;
    public bool LoggedIn { get; private set; } = false;

    /// <summary/>
    public MainPage()
    {
        BallSpinners.Add(new BallSpinnerViewModel(this, new Simulation()));

        InitializeComponent();

        BindingContext = this;
    }

    public void Init(FrontEnd frontEnd, IDatabase database)
    {
        _frontEnd = frontEnd;
        _database = database;

        _database.OnLoginChanged += Database_OnLoginChanged;
    }

    private void Database_OnLoginChanged(bool obj)
    {
        LoggedIn = obj;

        OnPropertyChanged(nameof(NotLoggedIn));
        OnPropertyChanged(nameof(LoggedIn));
    }

    public void RemoveBallSpinner(BallSpinnerViewModel ballSpinner)
    {
        BallSpinners.Remove(ballSpinner);
        ballSpinner.Dispose();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _database.OnLoginChanged -= Database_OnLoginChanged;

        foreach (var ballSpinner in BallSpinners)
        {
            ballSpinner.Dispose();
        }

        BallSpinners.Clear();
    }

    private void OnLoginButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.Login();
    }

    private void OnNewShotButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.InitialValues();
    }

    private void OnCloudManagementButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.CloudManagement();
    }

    private void OnLoadShotButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private async void OnSaveShotButtonClicked(object sender, EventArgs args)
    {
        if(await DisplayAlert("Save Shot as ...", "Save to database or local drive", "Database", "Local"))
        {
            string name = await DisplayPromptAsync("Notice", "Please name the test");

            if (name != null)
            {
                await _database.UploadShot(name, 0);
            }
        }
        else
        {
            string path = Utilities.GetTempDir();
            /*
            var dlg = new OpenFileDialog()
            {
                InitialDirectory = path,
                Filter = "Text Files (*.txt) | *.txt | All Files (*.*) | *.*",
                RestoreDirectory = true
            };
            */
            throw new NotImplementedException();
        }
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
            if(spinner.NotConnectedFadeVisible)
            {
                DisplayAlert("Can't start", "All ball spinners must be connected.", "Okay");
                return;
            }    
        }

        foreach (var spinner in BallSpinners)
        {
            spinner.Start();
        }
    }

    private async void OnStopButtonClicked(object sender, EventArgs args)
    {
        foreach (var spinner in BallSpinners)
        {
            spinner.Stop();
        }

        if(await DisplayAlert("Notice", "Would you like to save this throw", "Yes", "No"))
        {
            OnSaveShotButtonClicked(sender, args);
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
            BallSpinners.Add(new BallSpinnerViewModel(this, ballSpinner));
            OnPropertyChanged(nameof(BallSpinners));
        }
    }
}