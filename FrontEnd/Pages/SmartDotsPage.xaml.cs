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
        BindingContext = this;

        spinner.OnSmartDotMACAddressReceived += Spinner_OnSmartDotMACAddressReceived;
        spinner.ConnectSmartDot(null);
        BindingContext = MacAddresses;
	}

    private void Spinner_OnSmartDotMACAddressReceived(PhysicalAddress obj)
    {
        foreach (var address in MacAddresses)
        {
            if (address.MacAddress == obj)
                return; //Don't add duplicates
        }

        MacAddresses.Add(new MacAddressPair(obj, string.Empty));
    }

    public struct MacAddressPair
    {
        public PhysicalAddress MacAddress { get; }
        public string Name { get; } = string.Empty;

        public MacAddressPair(PhysicalAddress macAddress, string name)
        {
            MacAddress = macAddress;

            if (name.Length > 0)
                Name = $"{name} ({MacAddress.ToString()})";
            else
                Name = MacAddress.ToString();
        }
    }

    private void Connect_Clicked(object sender, EventArgs e)
    {
        if(SmartDots.SelectedItem is MacAddressPair pair)
        {
            _ballSpinner.ConnectSmartDot(pair.MacAddress);
            _task.SetResult(pair.MacAddress);
        }
    }
}