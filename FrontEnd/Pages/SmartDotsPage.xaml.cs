using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace RevMetrix.BallSpinner.FrontEnd.Pages;

public partial class SmartDotsPage : ContentPage
{
    public ObservableCollection<MacAddressPair> MacAddresses { get; } = new ObservableCollection<MacAddressPair>();

    private TaskCompletionSource<PhysicalAddress?> _task;
    private IBallSpinner _ballSpinner;

    public SmartDotsPage(IBallSpinner spinner, TaskCompletionSource<PhysicalAddress?> task)
	{
        _task = task;
        _ballSpinner = spinner;
        
        InitializeComponent();

        spinner.OnSmartDotMACAddressReceived += Spinner_OnSmartDotMACAddressReceived;
        spinner.ConnectSmartDot(null);
	}

    private void Spinner_OnSmartDotMACAddressReceived(PhysicalAddress obj)
    {
        foreach (var address in MacAddresses)
        {
            if (address.MacAddress == obj)
                return; //Don't add duplicates
        }

        MacAddresses.Add(new MacAddressPair(obj, string.Empty));
        OnPropertyChanged(nameof(MacAddresses));
    }

    public struct MacAddressPair
    {
        public PhysicalAddress MacAddress;
        public string Name;

        public MacAddressPair(PhysicalAddress macAddress, string name)
        {
            MacAddress = macAddress;

            if (name.Length > 0)
                Name = $"{name} ({MacAddress.ToString()})";
            else
                Name = macAddress.ToString();
        }
    }

    private void Connect_Clicked(object sender, EventArgs e)
    {
        if(SmartDots.SelectedItem != null)
        {
            /*_ballSpinner.ConnectSmartDot(obj);
            _task.SetResult(obj);*/
        }
    }
}