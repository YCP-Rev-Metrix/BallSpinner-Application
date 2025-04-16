using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Net;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class NewBallSpinnerView : ContentPage
{
	private TaskCompletionSource<IBallSpinner?> _task;

	private int _BallSpinnerCount;

	private int _SimulationCount;

    /// <summary>
    /// Initiates a new IBallSpinner of type Simulation or BallSpinnerClass based on user input
    /// </summary>
    /// <param name="task"> Returns a new instance of the user selected IBallSpinner</param>
    /// <param name="BallSpinnerCount">Represents the current number of BallSpinners open on the application. Used to assign file index.</param>
    /// <param name="SimulationCount">Represents the current number of BallSpinners open on the application. Used to assign file index.</param>
    public NewBallSpinnerView(TaskCompletionSource<IBallSpinner?> task, int BallSpinnerCount, int SimulationCount)
	{
		_task = task;

		_BallSpinnerCount = BallSpinnerCount;

		_SimulationCount = SimulationCount;

		InitializeComponent();
	}

	public async void AddSimulationButton(object sender, EventArgs args)
	{
		if (_SimulationCount < 1)
		{
            _task.SetResult(new Simulation());
        }
		else
		{
			await DisplayAlert("Alert", "You cannot add more than one simulation", "Okay");
			_task.SetResult(null);
		}
    }

	public async void AddBallSpinnerButton(object sender, EventArgs args)
	{
        if (_BallSpinnerCount < 1)
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
            await DisplayAlert("Alert", "You cannot add more than one Ball Spinner", "Okay");
            _task.SetResult(null);
        }
    }
}