using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Net;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class NewBallSpinnerView : ContentPage
{
	private TaskCompletionSource<IBallSpinner?> _task;
	private IBallSpinner _currentBallSpinner;

    public NewBallSpinnerView(TaskCompletionSource<IBallSpinner?> task, IBallSpinner ballSpinners)
	{
		_task = task;

		_currentBallSpinner = ballSpinners;

		InitializeComponent();
	}

	public void AddSimulationButton(object sender, EventArgs args)
	{
		// Only add a Spinner if one is not already added
        
        if (_currentBallSpinner == null || _currentBallSpinner?.GetType() == typeof(BallSpinnerClass))
        {
            _task.SetResult(new Simulation());
        } 
        else
        {
            _task.SetResult(null);
        }

    }

	public async void AddBallSpinnerButton(object sender, EventArgs args)
	{
        // Only add ball spinner if no ball spinners exist or if the only other ball spinner is a simulation
        if (_currentBallSpinner == null || _currentBallSpinner?.GetType() == typeof(Simulation))
        {
            var addr = IPAddr.Text;
            if (string.IsNullOrEmpty(addr))
            {
                await DisplayAlert("Alert", "You have not entered an IP address", "Fine");
            }
            else
            {
                try
                {
                    _task.SetResult(new BackEnd.BallSpinner.BallSpinnerClass(IPAddress.Parse(addr)));
                }
                catch (Exception e)
                {
                    await DisplayAlert("Alert", e.Message, "BOOOOOOOOOOOOOOOOOOOOOOOOO");
                }
            }
        }
        else
        {
            _task.SetResult(null);
        }
    }
}