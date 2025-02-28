using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Windows.Devices.Sensors;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class SmartDotSettingsPage : ContentPage
{
    private IBallSpinner ContextStore;
    private FrontEnd _frontEnd = null!;
    private BallSpinnerViewModel _ballSpinner = null!;
    private SmartDotSettingsViewModel _viewModel = null!;

    public SmartDotSettingsPage(FrontEnd frontEnd, BallSpinnerViewModel ballSpinner)
    {
        _frontEnd = frontEnd;
        _ballSpinner = ballSpinner;
        _viewModel = new SmartDotSettingsViewModel(ballSpinner.BallSpinner);

        /*
        ObservableCollection<List<double>> test = new ObservableCollection < List<double>>();
        test.Add(new List<double>());
        test.Add(new List<double>());
        test.Add(new List<double>());
        test.Add(new List<double>());
        test[0].Add(1);
        test[1].Add(2);
        test[2].Add(3);
        test[3].Add(4);

        BindingContext = test;
        */
        
        BindingContext = _viewModel;
        InitializeComponent();

        //Autopopulate the pickers to index 0 of their binding context
        AccelrationRange.SelectedIndex = 0;
        AccelrationSampleRate.SelectedIndex = 0;

        RotationRange.SelectedIndex = 0;
        RotationSampleRate.SelectedIndex = 0;

        MagnatometerRange.SelectedIndex = 0;
        MagnatometerSampleRate.SelectedIndex = 0;

        LightRange.SelectedIndex = 0;
        LightSampleRate.SelectedIndex = 0;
    }

    private void OnSubmitButtonClicked(object sender, EventArgs e)
	{
        double[] r = [_viewModel.Ranges[0][AccelrationRange.SelectedIndex], 
                      _viewModel.Ranges[1][RotationRange.SelectedIndex],
                      _viewModel.Ranges[2][MagnatometerRange.SelectedIndex],
                      _viewModel.Ranges[3][LightRange.SelectedIndex]];

        double[] sr = [_viewModel.SampleRates[0][AccelrationSampleRate.SelectedIndex],
                       _viewModel.SampleRates[1][RotationSampleRate.SelectedIndex],
                       _viewModel.SampleRates[2][MagnatometerSampleRate.SelectedIndex],
                       _viewModel.SampleRates[3][LightSampleRate.SelectedIndex]];

        _ballSpinner.BallSpinner.SubmitSmartDotConfig(r,sr,
                                                      _viewModel.Enables[0],
                                                      _viewModel.Enables[1],
                                                      _viewModel.Enables[2],
                                                      _viewModel.Enables[3]);

        _frontEnd.CloseSmartDotSettings();
	}

    private void OnEnabledToggleChanged(object sender, EventArgs e)
    {
        bool[] arr = [AccelerationEnabled.IsChecked, 
                      RotationEnabled.IsChecked,
                      MagnatometerEnabled.IsChecked, 
                      LightEnabled.IsChecked];

        _viewModel.BuildEnables(arr);
    }
}