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
    }

    private void OnSubmitButtonClicked(object sender, EventArgs e)
	{
		_frontEnd.CloseSmartDotSettings();
	}
}