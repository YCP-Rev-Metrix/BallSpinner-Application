using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class CloudManagementPage : ContentPage
{
    private ShotsViewModel ContextStore;
	public CloudManagementPage(IDatabase database)
	{
        InitializeComponent();
        ContextStore = new ShotsViewModel(database);
        BindingContext = ContextStore;
    }

    private void Refresh(object sender, EventArgs args)
    {
        //Temp hardcoded update, the function for refreshing on the ShotsViewModel side will replace this
        ContextStore.UpdateCollectionContent();

        BindingContext = ContextStore;
    }

    private void LoadSim(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void LoadInitial(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }
}