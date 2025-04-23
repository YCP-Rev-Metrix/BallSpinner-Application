using RevMetrix.BallSpinner.BackEnd;
using Common.POCOs;
using System.Runtime.CompilerServices;

namespace RevMetrix.BallSpinner.FrontEnd;

/********************************************************************************************
 *                                                                                          *
 *  x:Name of page elements                                                                 *
 *                                                                                          *
 *  Name        Type                Description                                             *
 *  ----------------------------------------------------------------------------------------*
 *                                                                                          *
 *******************************************************************************************/

public partial class CloudManagementPage : ContentPage
{
    private ShotsViewModel ContextStore;
    public SimulatedShot Selection = null;
    private int SortID = 0;
    IDatabase _database;
	public CloudManagementPage(IDatabase database)
	{
        InitializeComponent();
        SortType.SelectedIndex = 0;
        SortDir.SelectedIndex = 0;
        SortID = 0;
        _database = database;
        ContextStore = new ShotsViewModel(database);
        BindingContext = ContextStore;
    }

    private async void Refresh(object sender, EventArgs args)
    {
        await ContextStore.UpdateCollectionContent();
        ContextStore.SortCollection(SortID);
        BindingContext = ContextStore;
    }

    private void NewSelection(object sender, SelectedItemChangedEventArgs args)
    {
        Selection = args.SelectedItem as SimulatedShot;
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

    private async void DeleteShot(object sender, EventArgs args)
    {
        if (Selection == null)
        {
            DisplayAlert("Alert", "No shot selected", "Ok");
        }
        else if (await DisplayAlert("Alert", "Are you sure you want to delete " + Selection.shotinfo.Name, "Yes", "No"))
        {
            await _database.DeleteUserShot(Selection.shotinfo.Name);
            Refresh(sender, args);
        }
    }

    private void OnSortIndexChanged(object sender, EventArgs args)
    {
        SortID = (SortType.SelectedIndex * 2) + SortDir.SelectedIndex;
        if (ContextStore != null)
        {
            ContextStore.SortCollection(SortID);
            BindingContext = ContextStore;
        }
    }
}