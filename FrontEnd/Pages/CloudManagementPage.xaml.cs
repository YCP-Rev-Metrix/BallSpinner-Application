using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.FrontEnd;

public partial class CloudManagementPage : ContentPage
{
	public CloudManagementPage()
	{
        InitializeComponent();
        BindingContext = new ShotsViewModel();
    }

    private void LoadSim(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }

    private void LoadInitial(object sender, EventArgs args)
    {
        throw new NotImplementedException();
    }
}