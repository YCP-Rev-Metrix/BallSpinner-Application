using Common.POCOs;
using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Collections.ObjectModel;
using WinRT.FrontEndVtableClasses;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class InitialValues : ContentPage
{
    public FrontEnd _frontend;

    public ObservableCollection<BallSpinnerViewModel> _ballSpinners;

    public List<double> bezierPointsY;


    public InitialValues(FrontEnd frontend, ObservableCollection<BallSpinnerViewModel> ballSpinners)
	{
		InitializeComponent();

        _frontend = frontend;

        _ballSpinners = ballSpinners;

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
        // Send rpms to the all open ballspinners
        foreach (var BallSpinner in _ballSpinners)
        {
            BallSpinner.BallSpinner.SetInitialValues(bezierPointsY);
        }
    }
}