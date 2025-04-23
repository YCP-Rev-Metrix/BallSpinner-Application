using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class ArsenalPage : ContentPage
{
    private BallsViewModel ContextStore;
    public Ball Selection = null;
    IDatabase _database;
    public ArsenalPage(IDatabase database)
    {
        InitializeComponent();
        _database = database;
        ContextStore = new BallsViewModel(database);
        BindingContext = ContextStore;
    }
    
    private void Refresh(object sender, EventArgs args)
    {
        ContextStore.UpdateCollectionContent();
        BindingContext = ContextStore;
    }

    private void NewSelection(object sender, SelectedItemChangedEventArgs args)
    {
        Selection = args.SelectedItem as Ball;
    }

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

    private async void AddBall(object sender, EventArgs args)
    {
        if (   string.IsNullOrEmpty(Name.Text)
            || string.IsNullOrEmpty(Weight.Text)
            || CoreType.SelectedIndex == -1)
        {
            await DisplayAlert("Alert", "To add a new ball, every field must filled.", "Fine...");
        }
        else
        {
            try
            {
                await _database.AddBowlingBall(new Ball(Name.Text, 2, double.Parse(Weight.Text), (string?)CoreType.ItemsSource[CoreType.SelectedIndex]));
                Refresh(sender, args);
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.Message, "Ok");
            }
        }
    }
}