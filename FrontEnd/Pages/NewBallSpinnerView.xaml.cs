using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Net;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class NewBallSpinnerView : ContentPage
{
	private TaskCompletionSource<IBallSpinner?> _task;

    public NewBallSpinnerView(TaskCompletionSource<IBallSpinner?> task)
	{
		_task = task;

		InitializeComponent();
	}

	public void AddSimulationButton(object sender, EventArgs args)
	{
		_task.SetResult(new Simulation());
    }

	public async void AddBallSpinnerButton(object sender, EventArgs args)
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