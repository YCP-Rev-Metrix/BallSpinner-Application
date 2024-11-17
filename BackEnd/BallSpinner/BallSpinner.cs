using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
    public DataParser DataParser { get; } = new DataParser();

    /// <inheritdoc/>
    public event Action? SendErrorToApp;

    /// <inheritdoc/>
    public event Action? SendRejection;
    
    // TEMP variables. For now, used for automated motor instructions
    public int CurrentVoltage = 1;
    private static System.Timers.Timer aTimer;


    /// <inheritdoc/>
    public event Action<bool>? OnConnectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    private TCP? _connection;
    private IPAddress _address;

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
    }

    /// <inheritdoc/>
    public async void InitializeConnection()
    {
        if (IsConnected())
            return;

        _connection = new TCP(_address);
        await _connection.Connect();

        if (_connection.Connected)
            OnConnected();
        // Subscribe to smartDotRecieved event. Will trigger when a smartdot packet is received
        _connection.SmartDotRecievedEvent += SmartDotRecievedHandler;
    }

    private async void OnConnected()
    {
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Connected"));

        Name = await _connection!.GetDeviceInfo();
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Name)));

        var smartDot = await _connection.ConnectSmartDot();

        // Subscribe to OnDataRecievedEvent
        //_connection.OnDataRecieved += OnDataRecievedEventHandler;
        OnConnectionChanged?.Invoke(true);
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
        //message ball spinner
    }

    /// <inheritdoc/>
    public void Stop()
    {
        //message ball spinner
    }
    
    public void SendInstructions()
    {
        aTimer = new System.Timers.Timer(500);
        // Run event handler to send packet every 0.5 seconds. Make it repeat.
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }
    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        if (CurrentVoltage > 30)
        {
            // Kill the timer
            aTimer.Stop();
            aTimer.Dispose();
        }
        else {
            // FOR NOW! This is a script that will send automated instructions for MS2
            // This needs to be refactored to send predefined instructions based on kinematic calculations
            byte[] instructions = new byte[5];
            // Indicates a motor instruction packet
            instructions[0] = 0x88;
            // Message size
            instructions[1] = 0x03;
            //Motor 1 (main motor)
            instructions[2] = (byte) CurrentVoltage;
            //Motor 2
            instructions[3] = (byte) 2;
            //Motor 3
            instructions[4] = (byte) 2;
            // Send the motor instruction to the PI
            _connection.SendPacketToSmartDot(instructions);
            // Iterate current voltage for next iteration
            CurrentVoltage++;
            // Deallocate instructions
            instructions = null;
        }
    }
    /// <summary>
    /// Fires when TCP recieves a SmartDot packet
    /// </summary>
    public void SmartDotRecievedHandler(string[] SmartDotData, Metric[] Metrics, float XData, float YData, float ZData, float timeStamp)
    {
        DataParser.SendSmartDotToSubscribers(SmartDotData, Metrics, XData, YData, ZData, timeStamp);
    }
}
