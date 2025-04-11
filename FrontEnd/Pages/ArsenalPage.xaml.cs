using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

/********************************************************************************************
 *                                                                                          *
 *  x:Name of page elements                                                                 *
 *                                                                                          *
 *  Name        Type                Description                                             *
 *  ----------------------------------------------------------------------------------------*
 *  Name        Entry           -   name of the bowling ball for adding a new ball          *
 *  Diameter    Entry(Numeric)  -   diameter of the bowling ball for adding a new ball      *
 *  Weight      Entry(Numeric)  -   weight of the bowling ball for adding a new ball        *
 *  CoreType    Picker          -   The picker element for selecting the core type for      *
 *                                  adding a new bowling ball.                              *
 *                                                                                          *
 *******************************************************************************************/

public partial class ArsenalPage : ContentPage
{
    private BallsViewModel ContextStore;
    public Ball Selection = null;
    private int SortID;
    IDatabase _database;
    public ArsenalPage(IDatabase database)
    {
        InitializeComponent();
        SortType.SelectedIndex = 0;
        SortDir.SelectedIndex = 0;
        SortID = 0;
        _database = database;
        ContextStore = new BallsViewModel(database);
        BindingContext = ContextStore;
    }
    
    //Updates the content of the binding context
    private async void Refresh(object sender, EventArgs args)
    {
        await ContextStore.UpdateCollectionContent();
        ContextStore.SortCollection(SortID);
        BindingContext = ContextStore;
    }

    //Updates Selection
    private void NewSelection(object sender, SelectedItemChangedEventArgs args)
    {
        Selection = args.SelectedItem as Ball;
    }

    //Deletes selected shot
    private async void DeleteBall(object sender, EventArgs args)
    {
        if (Selection == null)
        {
            DisplayAlert("Alert", "No ball selected", "Ok");
        }
        else if (await DisplayAlert("Alert", "Are you sure you want to delete " + Selection.Name, "Yes", "No"))
        {
            await _database.DeleteBowlingBall(Selection.Name);
            Refresh(sender, args);
        }
    }


    //Adds a new bowling ball. 
    private async void AddBall(object sender, EventArgs args)
    {
        if (   string.IsNullOrEmpty(Name.Text)
            || string.IsNullOrEmpty(Diameter.Text)
            || string.IsNullOrEmpty(Weight.Text)
            || CoreType.SelectedIndex == -1)
        {
            await DisplayAlert("Alert", "To add a new ball, every field must filled.", "Fine...");
        }
        else
        {
            try
            {
                await _database.AddBowlingBall(new Ball(Name.Text, double.Parse(Diameter.Text), double.Parse(Weight.Text), (string?)CoreType.ItemsSource[CoreType.SelectedIndex]));
                Refresh(sender, args);
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.Message, "Ok");
            }
        }
    }

    private async void OnSortIndexChanged(object sender, EventArgs args)
    {
        SortID = (SortType.SelectedIndex * 2) + SortDir.SelectedIndex;
        if (ContextStore != null)
        {
            ContextStore.SortCollection(SortID);
            BindingContext = ContextStore;
        }
    }
}