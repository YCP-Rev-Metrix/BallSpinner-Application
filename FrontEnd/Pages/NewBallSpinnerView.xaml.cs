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

	public void AddBallSpinnerButton(object sender, EventArgs args)
	{
        _task.SetResult(new BackEnd.BallSpinner.BallSpinner(IPAddress.Parse("10.127.7.20")));
    }
}