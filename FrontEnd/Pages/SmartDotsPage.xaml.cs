using Common.POCOs;
using CsvHelper.Configuration.Attributes;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        Loaded += OnLoaded;
	}

    //TODO: Comment
    private void OnLoaded(object? sender, EventArgs e)
    {
        _ballSpinner.OnSmartDotMACAddressReceived += Spinner_OnSmartDotMACAddressReceived;
        _ballSpinner.ScanForSmartDots();
    }

    private void Spinner_OnSmartDotMACAddressReceived(PhysicalAddress obj)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            //Due to prexisting structures, we will wait to split the MAC address from the product name (sent in the same packet) at this point
            byte[] physAddrBytes = obj.GetAddressBytes();

            //grab the first 6 bytes which are the MAC address
            byte[] MACbytes = new byte[6];
            Array.Copy(physAddrBytes, MACbytes, 6);

            //grab the remaining bytes and turn them into ASCII
            byte[] BLENameBytes = new byte[physAddrBytes.Length - MACbytes.Length];
            Array.Copy(physAddrBytes, 6, BLENameBytes, 0, physAddrBytes.Length - MACbytes.Length);

            //This is the actual physical address with the ASCII BLEName bytes separated.
            var physicalAddress = new PhysicalAddress(MACbytes);

            Debug.WriteLine($"SmartDot MAC Address: {physicalAddress}");

            string BLEname = System.Text.Encoding.ASCII.GetString(BLENameBytes);
            Debug.WriteLine($"Name in ASCII {BLEname}");

            foreach (var address in MacAddresses)
            {
                if (address.MacAddress.Equals(physicalAddress))
                    return; //Don't add duplicates
            }
            MacAddresses.Add(new MacAddressPair(physicalAddress, BLEname));
        });
    }

    public partial class MacAddressPair
    {
        public PhysicalAddress MacAddress { get; }
        public string Name { get; private set; } = string.Empty;

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