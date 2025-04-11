using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Diagnostics;


namespace RevMetrix.BallSpinner.FrontEnd;

public partial class BallSpinnerView : ContentView
{
    private BallSpinnerViewModel _viewModel = null!;

	public BallSpinnerView()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        _viewModel = (BallSpinnerViewModel)BindingContext;
        //_viewModel.LeftView.DataReceived += (metric, value, timeFromStart) => { DataReceived(_viewModel.LeftView, LeftView, metric, value, timeFromStart); };
        _viewModel.TopMiddleView.DataReceived += (metric, value, timeFromStart) => { DataReceived(_viewModel.TopMiddleView, TopMiddleView, metric, value, timeFromStart); };
        _viewModel.TopRightView.DataReceived += (metric, value, timeFromStart) => { DataReceived(_viewModel.TopRightView, TopRightView, metric, value, timeFromStart); };
        _viewModel.BottomMiddleView.DataReceived += (metric, value, timeFromStart) => { DataReceived(_viewModel.BottomMiddleView, BottomMiddleView, metric, value, timeFromStart); };
        _viewModel.BottomRightView.DataReceived += (metric, value, timeFromStart) => { DataReceived(_viewModel.BottomRightView, BottomRightView, metric, value, timeFromStart); };
    }

    private void OnRemoveBallSpinnerButton(object sender, EventArgs args)
    {       
        _viewModel.MainPage.RemoveBallSpinner(_viewModel);
    }

    private void SelectSmartDotButton(object sender, EventArgs args)
    {
        _viewModel.ConnectSmartDot();
    }

    private void DataReceived(IDataViewModel model, WebView webview, Metric metric, float value, float timeFromStart)
    {
        if (!model.Metrics.HasFlag(metric))
            return;

        try
        {
            //Need to switch to a web connection, this is crazy slow!
            webview.Eval($"window.data('{metric}',{value}, {timeFromStart})");
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void Reconnect_Clicked(object sender, EventArgs e)
    {
        _viewModel.Reconnect();
    }

    private void OnSettingsButtonClicked(object sender, EventArgs e)
    {
        _viewModel.OpenSmartDotSettings();
    }
}