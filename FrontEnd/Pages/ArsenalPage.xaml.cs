using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class ArsenalPage : ContentPage
{
    private BallsViewModel ContextStore;

    public ArsenalPage(IDatabase database)
	{
		InitializeComponent();
        ContextStore = new BallsViewModel(database);
        BindingContext = ContextStore;
    }

    private void Refresh(object sender, EventArgs args)
    {
        ContextStore.UpdateCollectionContent();
        BindingContext = ContextStore;
    }
}