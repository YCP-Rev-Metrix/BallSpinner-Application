using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class SmartDotSettingsPage : ContentPage
{
    private IBallSpinner ContextStore;
    private FrontEnd _frontEnd = null!;
    private BallSpinnerViewModel _ballSpinner = null!;
    private SmartDotSettingsViewModel _viewModel = null;

    public SmartDotSettingsPage(FrontEnd frontEnd, BallSpinnerViewModel ballSpinner)
    {
        _frontEnd = frontEnd;
        _viewModel = new SmartDotSettingsViewModel(ballSpinner.BallSpinner);

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
		_frontEnd.CloseSmartDotSettings();
	}
}