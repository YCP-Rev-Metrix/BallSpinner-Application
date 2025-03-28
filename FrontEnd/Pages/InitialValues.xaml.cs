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

    private void PassValues(object sender, EventArgs args)
    {
        Console.Out.WriteLine("PassValues Running");
        Coordinates lower = new Coordinates(0, 0);
        Coordinates inflection = new Coordinates(70, 50);
        Coordinates upper = new Coordinates(100, MaxVal.Value);
        ContextStore.OnGraphChanged(lower, inflection, upper);
        BindingContext = ContextStore;
    }
}