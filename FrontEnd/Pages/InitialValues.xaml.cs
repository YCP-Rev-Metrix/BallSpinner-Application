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
        Coordinates preInflection = ContextStore.GetInflection();
        if (MaxVal.Value <= MinVal.Value) MaxVal.Value = MinVal.Value + 1;
        Coordinates lower = new Coordinates(0, MinVal.Value);
        Coordinates inflection = new Coordinates(70, ((MaxVal.Value - MinVal.Value) * (1.0 / 16.0) + MinVal.Value));
        //Coordinates inflection = new Coordinates(preInflection.x, ((MaxVal.Value-MinVal.Value)*(preInflection.y/(MaxVal.Value-MinVal.Value)))+MinVal.Value);
        Coordinates upper = new Coordinates(100, MaxVal.Value);
        ContextStore.OnGraphChanged(lower, inflection, upper);
        BindingContext = ContextStore;
    }

    private void OnMinSliderValueChanged(object sender, EventArgs args)
    {
        Coordinates preInflection = ContextStore.GetInflection();
        if (MinVal.Value >= MaxVal.Value) MinVal.Value = MaxVal.Value + 1;
        Coordinates lower = new Coordinates(0, MinVal.Value);
        Coordinates inflection = new Coordinates(70, ((MaxVal.Value - MinVal.Value) * (1.0 / 16.0) + MinVal.Value));
        //Coordinates inflection = new Coordinates(preInflection.x, ((MaxVal.Value-MinVal.Value)*(preInflection.y/(MaxVal.Value-MinVal.Value)))+MinVal.Value);
        Coordinates upper = new Coordinates(100, MaxVal.Value);
        ContextStore.OnGraphChanged(lower, inflection, upper);
        BindingContext = ContextStore;
    }

    private async void PassValues(object sender, EventArgs args)
    {
        // Get RPM values
            if (BallSelection.SelectedIndex == -1 || string.IsNullOrEmpty(Comment.Text))
            {
                await DisplayAlert("Alert", "No bowling ball selected or no Comment Made", "Ok");
            }
            else
            {
                // close Initial values window
                _frontend.CloseInitialValuesWindow();

                bezierPointsY.Clear();

                foreach (var _point in ContextStore.chart.bezierValues)
                {
                    bezierPointsY.Add(_point.Y);
                }



                // Send rpms to the all open ballspinners
                foreach (var BallSpinner in _ballSpinners)
                {

                    // Inflection is hardcoded for now
                    Coordinate BezierInitPoint = new Coordinate(0, MinVal.Value);
                    Coordinate BezierInflectionPoint = new Coordinate(70, 50);
                    Coordinate BezierFinalPoint = new Coordinate(100, MaxVal.Value);
                    Ball Ball = (Ball)BallSelection.SelectedItem;
                    string Comments = Comment.Text;
                    BallSpinner.BallSpinner.SetInitialValues(bezierPointsY, BezierInitPoint, BezierInflectionPoint, BezierFinalPoint, Comments, Ball);
                }
            }
        }
    }

