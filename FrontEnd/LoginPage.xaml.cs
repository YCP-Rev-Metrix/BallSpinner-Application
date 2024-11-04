using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class LoginPage : ContentPage
{
    private FrontEnd _frontEnd = null!;
    private IDatabase _database = null!;

    public LoginPage(FrontEnd frontEnd, IDatabase database)
	{
		InitializeComponent();

        _frontEnd = frontEnd;
        _database = database;

        BindingContext = this;
    }

    public void Init(FrontEnd frontEnd, IDatabase database)
    {
        _frontEnd = frontEnd;
        _database = database;
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs args)
    {
        var username = UsernameField.Text;
        var password = PasswordField.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Alert", "You have not entered either a username and/or password", "Fine");
        }
        else
        {
            try
            {
                var token = await _database.RegisterUser("a", "a", username, password, "a", "a");
                Console.WriteLine($"Auth: {token?.TokenA}");
                Console.WriteLine($"Refresh: {token?.TokenB}");
            }
            catch (Exception e)
            {
                await DisplayAlert("Alert", e.Message, "BOOOOOOOOOOOOOOOOOOOOOOOOO");
            }
        }
    }

    private async void OnLoginButtonClicked(object sender, EventArgs args)
    {
        string username = UsernameField.Text;
        string password = PasswordField.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Alert", "You have not entered either a username and/or password", "Fine");
        }
        else
        {
            try
            {
                var token = await _database.LoginUser(username, password);
                Console.WriteLine($"Auth: {token?.TokenA}");
                Console.WriteLine($"Refresh: {token?.TokenB}");

                //TODO: Close window
                await DisplayAlert("Success", "Login Successful", "Yipiee!");
            } 
            catch(Exception e) {
                await DisplayAlert("Alert", e.Message, "BOOOOOOOOOOOOOOOOOOOOOOOOO");
            }
        }
    }
}