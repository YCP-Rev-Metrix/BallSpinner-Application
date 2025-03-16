using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Net;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class NewBallSpinnerView : ContentPage
{
	private TaskCompletionSource<IBallSpinner?> _task;
	private IBallSpinner _ballSpinner;

    public NewBallSpinnerView(TaskCompletionSource<IBallSpinner?> task, IBallSpinner ballSpinners)
	{
		_task = task;

		_ballSpinner = ballSpinners;

		InitializeComponent();
	}

	public void AddSimulationButton(object sender, EventArgs args)
	{
		// Only add a Spinner if one is not already added
        if (_ballSpinner.GetType() == typeof(Simulation)) {

            _task.SetResult(null);
		}
		else
		{
            _task.SetResult(new Simulation());
        }
    }

	public async void AddBallSpinnerButton(object sender, EventArgs args)
	{
        // Only add a ball spinner if one is not already added
        if (_ballSpinner.GetType() == typeof(BallSpinnerClass))
        {
            DisplayAlert("NOPE", "Unable to add more than one Ball Spinner", "Okay");
            _task.SetResult(null);
        }
		else
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
    }
}