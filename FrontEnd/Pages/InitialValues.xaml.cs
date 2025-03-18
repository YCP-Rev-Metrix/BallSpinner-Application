using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class InitialValues : ContentPage
{
	public InitialValues()
	{
		InitializeComponent();

        
        InitialValuesModel model = new InitialValuesModel();
        Coordinates dummyvalues = new Coordinates(0, 0);
        List<List<double>> axes = model.CalculateBezierCurve(dummyvalues, dummyvalues, dummyvalues);
        var chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3]);
        BindingContext = chart;
        //BindingContext = this;
        for(int i = 0; i <= 100; i+=5) //delete this loop when done, currently it is for outputting bezier values to console.
        {
            Console.Out.WriteLine("X = " + axes[2][i] + " , Y = " + axes[3][i]);
        }
        

    }

    private void PassValues(object sender, EventArgs args)
    {
        
    }
}