using RevMetrix.BallSpinner.BackEnd.BallSpinner;

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
        _viewModel.LeftView.DataReceived += (metric, value, timeFromStart) => { DataReceived(LeftView, metric, value, timeFromStart); };
        _viewModel.TopMiddleView.DataReceived += (metric, value, timeFromStart) => { DataReceived(TopMiddleView, metric, value, timeFromStart); };
    }

    private void OnRemoveBallSpinnerButton(object sender, EventArgs args)
    {
        _viewModel.MainPage.RemoveBallSpinner(_viewModel);
    }

    private void DataReceived(WebView webview, Metric metric, float value, float timeFromStart)
    {
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
}