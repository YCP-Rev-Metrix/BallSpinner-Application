using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;


namespace RevMetrix.BallSpinner.FrontEnd;

public partial class InitialValues : ContentPage
{
    List<double> bezierPointsY;
    private BallSpinnerViewModel _ballSpinner;
    private BallSpinnerViewModel _simulation;
    private FrontEnd _frontend;
    public InitialValues(BallSpinnerViewModel ballSpinner, BallSpinnerViewModel simulation, FrontEnd frontend)
	{
        InitializeComponent();

        _ballSpinner = ballSpinner;
        _simulation = simulation;
        _frontend = frontend;
        
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

    private async void SetInitialValues(object sender, EventArgs args)
    {
        // close Initial values window
        _frontend.CloseInitialValuesWindow();
        // Send rpms to the Ball Spinner/simulation
        _ballSpinner.BallSpinner.SetMotorRPMs(bezierPointsY);
        _simulation.BallSpinner.SetMotorRPMs(bezierPointsY);
        //update initial values status
        _ballSpinner.InitialValuesCheck = true;
        _simulation.InitialValuesCheck = true;
    }
}