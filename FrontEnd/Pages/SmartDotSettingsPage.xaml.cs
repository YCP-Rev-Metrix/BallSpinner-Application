namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class SmartDotSettingsPage : ContentPage
{
	private FrontEnd _frontEnd = null!;
    private BallSpinnerViewModel _viewModel = null!;

    public SmartDotSettingsPage(FrontEnd frontEnd, BallSpinnerViewModel viewModel)
    {
        _frontEnd = frontEnd;
        _viewModel = viewModel;

        InitializeComponent();
        _viewModel = viewModel;
    }

    private void OnSubmitButtonClicked(object sender, EventArgs e)
	{
		_frontEnd.CloseSmartDotSettings();
	}
}