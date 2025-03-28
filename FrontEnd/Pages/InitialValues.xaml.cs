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
        //TODO
    }
}