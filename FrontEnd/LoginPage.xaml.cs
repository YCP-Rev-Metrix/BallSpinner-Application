using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class LoginPage : ContentPage
{
    private FrontEnd _frontEnd = null!;
    private IDatabase _database = null!;

    public LoginPage()
	{
		InitializeComponent();

        BindingContext = this;
    }

    public void Init(FrontEnd frontEnd, IDatabase database)
    {
        _frontEnd = frontEnd;
        _database = database;
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs args)
    {
        var firstname = RegisterFirstName.Text;
        var lastname = RegisterLastName.Text;
        var username = RegisterUsername.Text;
        var password = RegisterPassword.Text;
        var email = RegisterEmail.Text;
        var phone = RegisterPhone.Text;
        var token = await _database.RegisterUser(firstname, lastname, username, password, email, phone);
        Console.WriteLine($"Auth: {token?.TokenA}");
        Console.WriteLine($"Refresh: {token?.TokenB}");
    }

    private async void OnLoginButtonClicked(object sender, EventArgs args)
    {
        var username = LoginUsername.Text;
        var password = LoginPassword.Text;

        var token = await _database.LoginUser(username, password);
        Console.WriteLine($"Auth: {token?.TokenA}");
        Console.WriteLine($"Refresh: {token?.TokenB}");
    }
}