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

    public List<double?> bezierPointsY;


    private InitialValuesViewModel ContextStore;

    public InitialValues(FrontEnd frontend, ObservableCollection<BallSpinnerViewModel> ballSpinners, IDatabase database)
	{
        ContextStore = new InitialValuesViewModel(database);
        BindingContext = ContextStore;

        InitializeComponent();

        MaxVal.Value = 800;
    

       _frontend = frontend;

       _ballSpinners = ballSpinners;

        bezierPointsY = new List<double?>();

    }

    private void OnMaxSliderValueChanged(object sender, EventArgs args)
    {
        if (MaxVal.Value <= MinVal.Value) MaxVal.Value = MinVal.Value + 1;
    }

    private void OnMinSliderValueChanged(object sender, EventArgs args)
    {
        if (MinVal.Value >= MaxVal.Value) MinVal.Value = MaxVal.Value + 1;
    }

    private async void PassValues(object sender, EventArgs args)
    {
        if (BallSelection.SelectedIndex == -1 || string.IsNullOrEmpty(Comment.Text))
        {
            await DisplayAlert("Alert", "No bowling ball selected or no Comment Made", "Ok");
        } else {
            // close Initial values window
            _frontend.CloseInitialValuesWindow();

            // Get RPM values
            foreach (var _point in ContextStore.chart.bezierValues)
            {
                bezierPointsY.Add(_point.Y);
            }

            // Send rpms to the all open ballspinners
            foreach (var BallSpinner in _ballSpinners)
            {

                // Hardcoded coordinates for now
                Coordinate BezierInitPoint = new Coordinate(0, 0);
                Coordinate BezierInflectionPoint = new Coordinate(1.2, 233);
                Coordinate BezierFinalPoint = new Coordinate(2.9, 775);
                Ball Ball = (Ball)BallSelection.SelectedItem;
                string Comments = Comment.Text;
                BallSpinner.BallSpinner.SetInitialValues(bezierPointsY, BezierInitPoint, BezierInflectionPoint, BezierFinalPoint, Comments, Ball);
            }
        }
    }
}