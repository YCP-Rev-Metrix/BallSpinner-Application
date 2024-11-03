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

    private void OnLoginButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnRegisterButtonClicked(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }
}