using Common.POCOs;
using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class InitialValues : ContentPage
{
    private InitialValuesViewModel ContextStore;

    public InitialValues(IDatabase database)
	{
        ContextStore = new InitialValuesViewModel(database);
        BindingContext = ContextStore;
        
        InitializeComponent();

        MaxVal.Value = 800;
    }

    private void OnMaxSliderValueChanged(object sender, EventArgs args)
    {
        if (MaxVal.Value <= MinVal.Value) MaxVal.Value = MinVal.Value + 1;
        Coordinates lower = new Coordinates(0, MinValue.Value);
        Coordinates inflection = new Coordinates(70, 50);
        Coordinates upper = new Coordinates(100, MaxVal.Value);
        ContextStore.OnGraphChanged(lower, inflection, upper);
        BindingContext = ContextStore;
    }

    private void OnMinSliderValueChanged(object sender, EventArgs args)
    {
        if (MinVal.Value >= MaxVal.Value) MinVal.Value = MaxVal.Value + 1;
    }

    private async void PassValues(object sender, EventArgs args)
    {
        //TODO
    }
}