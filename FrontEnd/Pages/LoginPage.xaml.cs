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
        string firstName = RegisterFirstNameField.Text;
        string lastName = RegisterLastNameField.Text;
        string email = RegisterEmailField.Text;
        string phoneNum = RegisterPhoneNumField.Text;
        string username = RegisterUsernameField.Text;
        string password = RegisterPasswordField.Text;
        string confirmPassword = RegisterConfirmPasswordField.Text;
        int dummyInt;

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
        else if (password.CompareTo(confirmPassword) != 0)
        {
            await DisplayAlert("Alert", "Passwords do not match", "UGHHHHHHHHH");
        }
        else if (!email.Contains('@'))
        {
            await DisplayAlert("Alert", "Improper email", "UGHHHHHHHHH");
        }
        else if (phoneNum.Length != 10 || !phoneNum.All(char.IsDigit))
        {
            await DisplayAlert("Alert", "Improper phone number", "UGHHHHHHHHH");
        }
        else
        {
            try
            {

                var token = await _database.RegisterUser(firstName, lastName, username, password, email, phoneNum);
                Console.WriteLine($"Auth: {token?.TokenA}");
                Console.WriteLine($"Refresh: {token?.TokenB}");

                RegisterFirstNameField.Text = null;
                RegisterLastNameField.Text = null;
                RegisterEmailField.Text = null;
                RegisterPhoneNumField.Text = null;
                RegisterUsernameField.Text = null;
                RegisterPasswordField.Text = null;
                RegisterConfirmPasswordField.Text = null;

                await DisplayAlert("Alert", "User successfully created", "Woooooo");
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