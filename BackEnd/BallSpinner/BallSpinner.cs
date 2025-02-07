using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;

/// <summary>
/// The real, physical ball spinner device
/// </summary>
public class BallSpinner : IBallSpinner
{
    /// <inheritdoc/>
    public string Name { get; set; } = "Real Device";

    /// <inheritdoc/>
    public string MAC { get; set; } = "TBD";

    /// <inheritdoc/>
    public DataParser DataParser { get; private set; } = new DataParser();

    public string SmartDotMAC { get; }

    /// <inheritdoc/>
    public event Action? SendErrorToApp;

    /// <inheritdoc/>
    public event Action? SendRejection;

    /// <inheritdoc/>
    public event Action<bool>? OnConnectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event Action<PhysicalAddress> OnSmartDotMACAddressReceived;

    private TCP? _connection;
    private IPAddress _address;

    private byte _currentVoltage = 0;
    private Timer? _motorTimer;

    /// <summary />
    public BallSpinner(IPAddress address)
    {
        _address = address;
        InitializeConnection();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;

        _motorTimer?.Dispose();
        _motorTimer = null;
    }

    /// <inheritdoc/>
    public async void InitializeConnection()
    {
        if (IsConnected())
            return;

        _connection?.Dispose();
        _connection = new TCP(_address);
        await _connection.Connect();
        
        if (_connection != null && _connection.Connected)
            OnConnected();
    }

    private async void OnConnected()
    {
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Connected"));

        Name = await _connection!.GetDeviceInfo();
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Name)));

        _connection!.ConnectSmartDot(null);
        _connection.SmartDotAddressReceivedEvent += SmartDotAddressReceivedEvent;

        // Subscribe to smartDotRecieved event. Will trigger when a smartdot packet is received
        _connection.SmartDotReceivedEvent += SmartDotRecievedEvent;

        OnConnectionChanged?.Invoke(true);
    }

    /// <inheritdoc/>
    public async void ConnectSmartDot(PhysicalAddress? address)
    {
        await _connection!.ConnectSmartDot(address);
    }

    private void SmartDotAddressReceivedEvent(PhysicalAddress address)
    {
        Debug.WriteLine("Device address: " + address.ToString() + " This has extra bytes from the product name");
        OnSmartDotMACAddressReceived?.Invoke(address);
    }

    private void SmartDotRecievedEvent(SensorType sensorType, float timeStamp, int sampleCount, float XData, float YData, float ZData)
    {
        DataParser.SendSmartDotToSubscribers(sensorType, timeStamp, sampleCount, XData, YData, ZData);
    }

    /// <inheritdoc/>
    public bool IsConnected()
    {
        return _connection != null && _connection.Connected;
    }

    /// <inheritdoc/>
    public void ResendMessage()
    {
        
    }

    /// <inheritdoc/>
    public List<string> SendBackListOfSmartDots()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetSmartDot()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Start()
    {
        Stop();

        DataParser.Start(Name);

        _currentVoltage = 1;
        _motorTimer = new Timer(TimeSpan.FromSeconds(0.25));
        _motorTimer.Elapsed += OnTimedEvent;
        _motorTimer.Start();
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _motorTimer?.Dispose();
        _motorTimer = null;

        _connection?.SetMotorVoltages(0, 0, 0);

        DataParser.Stop();
    }

    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        if (!IsConnected())
            return;

        _currentVoltage++;

        if (_currentVoltage >= 30) //Primary motor supports up to 30, secondary motors only 12
            _currentVoltage = 0;

        byte x, y, z;

        if (_currentVoltage >= 0 && _currentVoltage <= 10)
            y = 8;
        else
            y = 0;

        if (_currentVoltage >= 10 && _currentVoltage <= 20)
            z = 8;
        else
            z = 0;

        if (_currentVoltage >= 15 && _currentVoltage <= 30)
            x = _currentVoltage;
        else
            x = 0;

        //x = 30;

        _connection!.SetMotorVoltages(x, y, z);
    }
}
