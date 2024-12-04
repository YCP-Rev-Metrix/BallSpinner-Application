using RevMetrix.BallSpinner.BackEnd;
using Common.POCOs;
using System.Runtime.CompilerServices;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class CloudManagementPage : ContentPage
{
    private ShotsViewModel ContextStore;
    public SimulatedShot Selection = null;
    IDatabase _database;
	public CloudManagementPage(IDatabase database)
	{
        InitializeComponent();
        _database = database;
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
        if (Selection == null)
        {
            DisplayAlert("Alert", "No shot selected", "Ok");
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void LoadInitial(object sender, EventArgs args)
    {
        if (Selection == null) 
        {
            DisplayAlert("Alert", "No shot selected", "Ok");
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void NewSelection(object sender, SelectedItemChangedEventArgs args)
    {
        Selection = args.SelectedItem as SimulatedShot;
    }

    private async void DeleteShot(object sender, EventArgs args)
    {
        if (Selection == null)
        {
            DisplayAlert("Alert", "No shot selected", "Ok");
        }
        else if (await DisplayAlert("Alert", "Are you sure you want to delete " + Selection.simulatedShot.Name, "Yes", "No"))
        {
            // Kill the shot
            Refresh(sender, args);
        }
    }
}