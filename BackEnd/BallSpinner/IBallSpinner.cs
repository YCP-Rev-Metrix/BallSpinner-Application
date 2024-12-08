using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;

/// <summary>
/// interface defining core functionality for interacting with the ball spinner 
/// </summary>
public interface IBallSpinner : INotifyPropertyChanged
{

    /// <summary>
    /// Gets or sets the name of the Ball Spinner device.
    /// </summary>
    string Name { get; set;}

    /// <summary>
    /// Gets or sets the MAC address
    /// </summary>
    string MAC { get; }


    /// <summary>
    /// The MAC address of the Smart Dot Module
    /// </summary>
    string SmartDotMAC { get; }

    /// <summary>
    /// Events
    /// Event triggered when data is received from the Ball Spinner.
    /// </summary>
    DataParser DataParser { get; }

    /// <summary>
    ///Event triggered when an error message is received from the Ball Spinner.
    /// </summary>
    event Action? SendErrorToApp;

    /// <summary>
    /// Event triggered when a rejection message is received from the Ball Spinner.
    /// </summary>
    event Action? SendRejection;

    /// <summary>
    /// Invoked when the device is connected or disconnected
    /// </summary>
    public event Action<bool>? OnConnectionChanged;

    /// <summary>
    /// Invoked when the device receives a signal for a potential smart dot connection
    /// </summary>
    public event Action<PhysicalAddress> OnSmartDotMACAddressReceived;

    //Methods
    /// <summary>
    /// Disposes of resources used by the Ball Spinner.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Initializes the connection to the Ball Spinner device.
    /// </summary>
    void InitializeConnection();

    /// <summary>
    /// Starts the Ball Spinner device, beginning any data transmission.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the Ball Spinner device, ending any data transmission.
    /// </summary>
    void Stop();

    /// <summary>
    /// Checks if the connection to the Ball Spinner device is established.
    /// </summary>
    /// <returns>True if connected, false otherwise.</returns>
    bool IsConnected();

    /// <summary>
    /// Connect the device to a particular smart dot
    /// When null, tells the device to request for potential smart dot devices
    /// </summary>
    void ConnectSmartDot(PhysicalAddress? address);
}

/// <summary>
/// Flags of all types of metrics a ball spinner can output
/// </summary>
[Flags]
public enum Metric
{
    /// <summary/>
    None = 0,

    /// <summary/>
    MagnetometerX = 1 << 0,
    /// <summary/>
    MagnetometerY = 1 << 1,
    /// <summary/>
    MagnetometerZ = 1 << 2,

    /// <summary/>
    RotationX = 1 << 3,
    /// <summary/>
    RotationY = 1 << 4,
    /// <summary/>
    RotationZ = 1 << 5,

    /// <summary/>
    AccelerationX = 1 << 6,
    /// <summary/>
    AccelerationY = 1 << 7,
    /// <summary/>
    AccelerationZ = 1 << 8,

    /// <summary/>
    Light = 1 << 9
}