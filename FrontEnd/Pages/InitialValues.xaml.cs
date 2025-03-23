using Common.POCOs;
using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class InitialValues : ContentPage
{
    public FrontEnd _frontend;

    public BallSpinnerClass _ballSpinner;

    public List<double> bezierPointsY;


    public InitialValues(FrontEnd frontend, BallSpinnerClass ballSpinner)
	{
		InitializeComponent();

        _frontend = frontend;

        _ballSpinner = ballSpinner;

        InitialValuesModel model = new InitialValuesModel();
        Coordinates dummyvalues = new Coordinates(0, 0);
        List<List<double>> axes = model.CalcuateBezierCruve(dummyvalues, dummyvalues, dummyvalues);
        var chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3]);
        bezierPointsY = axes[3];
        BindingContext = chart;
        //BindingContext = this;
        

    }

    private async void SetInitialValues(object sender, EventArgs args)
    {
        // close Initial values window
        _frontend.CloseInitialValuesWindow();
        // Send rpms to the Ball Spinner/simulation
        _ballSpinner.SetMotorRPMs(bezierPointsY);
        //_simulation.BallSpinner.SetMotorRPMs(bezierPointsY);
    }
}