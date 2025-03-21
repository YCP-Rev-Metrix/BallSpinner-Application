using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;
using RevMetrix.BallSpinner.BackEnd.Database;
using System.Collections.ObjectModel;
using WinRT.FrontEndVtableClasses;
using System.ComponentModel;
using System.Threading.Tasks;

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
    /// <summary/>
    public MainPage()
    {
        InitializeComponent();

        //BindingContext = new RevMetrix.BallSpinner.FrontEnd.TestChart2();
        BindingContext = this;
    }

    public void Init(FrontEnd frontEnd, IDatabase database)
    {
        _frontEnd = frontEnd;
        _database = database;

        _database.OnLoginChanged += Database_OnLoginChanged;

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

    private async void OnNewShotButtonClicked(object sender, EventArgs args)
    {
        // Extract BallSpinnerClass instance
        BallSpinnerViewModel? ballSpinner = null;
        BallSpinnerViewModel? simulation = null;
        foreach(var _ballSpinner in BallSpinners)
        {
            if (_ballSpinner.BallSpinner.GetType() == typeof(BallSpinnerClass)) 
            {
                ballSpinner = _ballSpinner;
            }
            else if (_ballSpinner.BallSpinner.GetType() == typeof(Simulation))
            {
                simulation = _ballSpinner;
            }
        }
        // If the user is not properly connected to a ball spinner instance, return an error message
        if (ballSpinner == null || (ballSpinner != null && !ballSpinner.BallSpinner.IsConnected()))
        {
            await DisplayAlert("Not connected to a Ball Spinner", "Please connect to a Ball Spinner to enter initial values", "Ok");
            return;
        }

        // If the user is not properly connected to a ball spinner instance, return an error message
        if (simulation == null)
        {
            await DisplayAlert("Not connected to a Simulation", "Please open a simulation to enter initial values", "Ok");
            return;
        }

        _frontEnd.InitialValues(ballSpinner, simulation);
    }

    private void OnCloudManagementButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.CloudManagement();
    }

    private async void OnLoadShotButtonClicked(object sender, EventArgs args)
    {
        var customFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".csv"} }, // file extension
                { DevicePlatform.macOS, new[] { "csv"} } // UTType values
            });
        
        PickOptions options = new()
        {
            PickerTitle = "Please select a csv file",
            FileTypes = customFileType,
        };
        FileResult result = await FilePicker.Default.PickAsync(options);
        /*
        if (result != null)
        {
            // Create new ballSpinner instance, display on screen
            PreviousThrow ballSpinner = new PreviousThrow(result.FullPath);
            BallSpinnerViewModel viewModel = new BallSpinnerViewModel(_frontEnd, this, ballSpinner);

            BallSpinners.Add(viewModel);
            OnPropertyChanged(nameof(BallSpinners));
        }
        */
    }

    private async void OnSaveShotButtonClicked(object sender, EventArgs args)
    {
        foreach (var ballSpinner in BallSpinners)
        {
            if (ballSpinner.BallSpinner.GetType() != typeof(PreviousThrow))
            {
                if (await DisplayAlert("Notice", $"Would you like to save shot for device '{ballSpinner.Name}'?", "Yes", "No"))
                {
                    if (await DisplayAlert("Save Shot as ...", $"For device '{ballSpinner.Name}', save to database or local drive?", "Database", "Local"))
                    {
                        string name = await DisplayPromptAsync("Notice", "Please name the test");

                        if (name != null)
                        {
                            await _database.UploadShot(ballSpinner.BallSpinner, name, 0);
                        } 
                    }
                    else
                    {
                        // local option selected
                        string name = await DisplayPromptAsync("Notice", "Please name your file");
                    
                        if (name != null)
                        {
                            try
                            {
                                Utilities.SaveLocalRevFile(name, ballSpinner.Name, Environment.GetEnvironmentVariable("CurrentUser"));
                                // local save was successful, save this new local entry to database
                                // await _database.SaveLocalEntry(name); - For now this will not work. Endpoint not implemented on cloud
                            }
                            catch (Exception e)
                            {
                                await DisplayAlert("Error When Attempting to Save", e.Message, "Ok");
                            }
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
        foreach (var spinner in BallSpinners)
        {
            if(spinner.NotConnectedFadeVisible || !spinner.InitialValuesSet)
            {
                DisplayAlert("Can't start", "All ball spinners must be connected/have initial values.", "Okay");
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

    private async void OnAddBallSpinnerButtonClicked(object sender, EventArgs args)
    {
        IBallSpinner _ballSpinner = null;
        // Dont allow the user to add a new ball spinner if there are already two ball spinner present
        if (BallSpinners.Count >= 2)
        {
            await DisplayAlert("Cannot add a new Ball Spinner", "Limit already exceeded", "Fine!");
            return;
        }
        else if (BallSpinners.Count == 1)
        {
            _ballSpinner = BallSpinners[0].BallSpinner;
        }

        var ballSpinner = await _frontEnd.AddBallSpinner(_ballSpinner);

        if(ballSpinner != null)
        {
            BallSpinners.Add(new BallSpinnerViewModel(_frontEnd, this, ballSpinner));
            OnPropertyChanged(nameof(BallSpinners));
        }
        else
        {
            await DisplayAlert("Error", "Cannot add more than one type of Ball Spinner", "Okay");
        }
    }

    private void OnArsenalButtonClicked(object sender, EventArgs args)
    {
        _frontEnd.Arsenal();
    }
}