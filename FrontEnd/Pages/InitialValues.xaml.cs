using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class InitialValues : ContentPage
{
    List<double> bezierPointsY;
    private BallSpinnerClass _ballSpinner;
    public InitialValues(BallSpinnerClass ballSpinner)
	{
        InitializeComponent();

        _ballSpinner = ballSpinner;
        
        InitialValuesModel model = new InitialValuesModel();
        Coordinates dummyvalues = new Coordinates(0, 0);
        List<List<double>> axes = model.CalcuateBezierCruve(dummyvalues, dummyvalues, dummyvalues);
        // Set beginning bezier y for motor instructions
        bezierPointsY = axes[3];
        var chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3]);
        BindingContext = chart;
        //BindingContext = this;
        for(int i = 0; i <= 100; i+=5) //delete this loop when done, currently it is for outputting bezier values to console.
        {
            Console.Out.WriteLine("X = " + axes[2][i] + " , Y = " + axes[3][i]);
        }
    }

    private async void SendMotorInstructions(object sender, EventArgs args)
    {
        // Send rpms to the Ball Spinner
        _ballSpinner.SendMotorRPMs(bezierPointsY);
    }
}