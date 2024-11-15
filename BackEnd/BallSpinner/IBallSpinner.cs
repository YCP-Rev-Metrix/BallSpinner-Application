using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    string MAC { get; set; }

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
    /// Sets the active SmartDot on the Ball Spinner device.
    /// </summary>
    void SetSmartDot();

    /// <summary>
    /// Starts the Ball Spinner device, beginning any data transmission.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the Ball Spinner device, ending any data transmission.
    /// </summary>
    void Stop();

    /// <summary>
    /// Sends a list of SmartDots back to the connected application.
    /// </summary>
    /// <returns>A list of SmartDot names as strings.</returns>
    List<string> SendBackListOfSmartDots();

    /// <summary>
    /// Resends the last message if needed.
    /// </summary>
    void ResendMessage();

    /// <summary>
    /// Checks if the connection to the Ball Spinner device is established.
    /// </summary>
    /// <returns>True if connected, false otherwise.</returns>
    bool IsConnected();


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