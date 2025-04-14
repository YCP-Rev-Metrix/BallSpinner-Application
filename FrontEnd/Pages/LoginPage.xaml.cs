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

    private async void OnRegisterButtonClicked(object sender, EventArgs args)
    {
        var firstName = RegisterFirstNameField.Text;
        var lastName = RegisterLastNameField.Text;
        var email = RegisterEmailField.Text;
        var phoneNum = RegisterPhoneNumField.Text;
        var username = RegisterUsernameField.Text;
        var password = RegisterPasswordField.Text;
        var confirmPassword = RegisterConfirmPasswordField.Text;

        if (string.IsNullOrEmpty(firstName) || 
            string.IsNullOrEmpty(lastName)  || 
            string.IsNullOrEmpty(email)     || 
            string.IsNullOrEmpty(phoneNum)  || 
            string.IsNullOrEmpty(username)  || 
            string.IsNullOrEmpty(password)  ||
            string.IsNullOrEmpty(confirmPassword))
        {
            await DisplayAlert("Alert", "You have not entered compleated all fields", "Fine");
        }
        else if (RegisterPasswordField.Text.CompareTo(RegisterConfirmPasswordField.Text) != 0)
        {
            await DisplayAlert("Alert", "Passwords do not match", "UGHHHHHHHHH");
        }
        {
            try
            {
                var token = await _database.RegisterUser(firstName, lastName, username, password, email, phoneNum);
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
        string username = LoginUsernameField.Text;
        string password = LoginPasswordField.Text;

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

                _frontEnd.CloseLoginWindow();
            } 
            catch(Exception e) {
                await DisplayAlert("Alert", e.Message, "BOOOOOOOOOOOOOOOOOOOOOOOOO");
            }
        }
    }
}