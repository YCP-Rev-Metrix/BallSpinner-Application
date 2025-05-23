using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;
using RevMetrix.BallSpinner.BackEnd.Database;
using System.Collections.ObjectModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// View for the main page
/// </summary>
public partial class MainPage : ContentPage
{
    private FrontEnd _frontEnd = null!;
    private IDatabase _database = null!;
    public ISeries[] AccelerationChartValues { get; set; }
    public ISeries[] RotationChartValues{ get; set; }
    public ISeries[] MagnetometerChartValues { get; set; }
    public ISeries[] LightChartValues { get; set; }
    public ObservableCollection<BallSpinnerViewModel> BallSpinners { get; } = new();

    public bool NotLoggedIn => !LoggedIn;
    public bool LoggedIn { get; private set; } = false;

    // Counts the number of currently open simulations/ballspinners.
    public int SimulationCount { get; set; } = 1; // defaults to 1 because a simulation is opened by default

    public int BallSpinnerCount { get; set; } = 0;
    /// <summary/>
    public MainPage()
    {
        InitializeComponent();

        //BindingContext = new RevMetrix.BallSpinner.FrontEnd.TestChart2();
        BindingContext = this;
        //using (OpenTKGame game = new OpenTKGame(800, 600, "LearnOpenTK"))
        //{
        //    game.Run();
        //}
    }

    public void Init(FrontEnd frontEnd, IDatabase database)
    {
        _frontEnd = frontEnd;
        _database = database;

        _database.OnLoginChanged += Database_OnLoginChanged;
        //Add simulation by default
        BallSpinners.Add(new BallSpinnerViewModel(_frontEnd, this, new Simulation()));
    }

    private void Database_OnLoginChanged(bool obj)
    {
        LoggedIn = obj;

        OnPropertyChanged(nameof(NotLoggedIn));
        OnPropertyChanged(nameof(LoggedIn));
    }

    public void RemoveBallSpinner(BallSpinnerViewModel ballSpinner)
    {
        // Remove instance, and decrement count
        if (ballSpinner.BallSpinner.GetType() == typeof(BallSpinnerClass))
        {
            BallSpinnerCount--;
        }
        else if (ballSpinner.BallSpinner.GetType() == typeof(Simulation))
        {
            SimulationCount--;
        }
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
        // Check to see if a user has any open IBallSpinner instances
        if (BallSpinners.Count == 0)
        {
            DisplayAlert("Alert", "Please open a Ball Spinner to enter initial values.", "Ok");
            return;
        }
        // Load initial values page passing in an instance of all open BallSpinners
        _frontEnd.InitialValues(BallSpinners);
    }

    private void OnCloudManagementButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.CloudManagement();
    }

    private async void OnLoadShotButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException("Need to change this so that it uses the cloud database/local database");
    }

    private async void OnSaveShotButtonClicked(object sender, EventArgs args)
    {
        foreach (var ballSpinner in BallSpinners)
        {
            if (ballSpinner.BallSpinner.GetType() != typeof(PreviousThrow))
            {
                if (await DisplayAlert("Notice", $"Would you like to save shot for device '{ballSpinner.Name}'?", "Yes", "No"))
                {
                    string name = await DisplayPromptAsync("Notice", "Please name the shot");

                    if (name != null)
                    {
                        try
                        {
                            await _database.UploadShot(ballSpinner.BallSpinner, name, 0);
                        }
                        catch (Exception e)
                        {
                            await DisplayAlert("Error", e.Message, "Okay");
                        }
                    }
                }
            }
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
        // check to see if all ballspinners are good to go
        foreach (var spinner in BallSpinners)
        {
            if(spinner.NotConnectedFadeVisible || !spinner.InitialValuesSet)
            {
                DisplayAlert("Can't start", "All ball spinners must be connected and have initial values set.", "Okay");
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

        OnSaveShotButtonClicked(sender, args);
    }

    private void OnResetButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Gives user the option to open a new IBallSpinner instance. If a new IBallSpinner instance is selected, BallSpinnerCount and SimulationCount must be updated here to keep counts up to date.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private async void OnAddBallSpinnerButtonClicked(object sender, EventArgs args)
    {
        if (BallSpinnerCount >= 1 && SimulationCount >= 1)
        {
            await DisplayAlert("Alert", "You have reached the limit for the number of IBallSpinner instances open", "Okay");
            return;
        }
        var ballSpinner = await _frontEnd.AddBallSpinner(BallSpinnerCount, SimulationCount);

        if(ballSpinner != null)
        {
            if (ballSpinner.GetType() == typeof(BallSpinnerClass))
            {
                BallSpinnerCount++;
            }
            else if (ballSpinner.GetType() == typeof(Simulation))
            {
                SimulationCount++;
            }
            BallSpinners.Add(new BallSpinnerViewModel(_frontEnd, this, ballSpinner));
            OnPropertyChanged(nameof(BallSpinners));
        }
    }

    private void OnArsenalButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.Arsenal();
    }
}