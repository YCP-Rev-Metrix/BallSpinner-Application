using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;

/// <summary>
/// interface defining core functionality for interacting with the ball spinner 
/// </summary>
public interface IBallSpinner
{

    /// <summary>
    /// Properties 
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
    event Action DataParser;

    /// <summary>
    ///Event triggered when an error message is received from the Ball Spinner.
    /// </summary>
    event Action sendErrorToApp;

    /// <summary>
    /// Event triggered when a rejection message is received from the Ball Spinner.
    /// </summary>
    event Action sendRejection;

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
    void setSmartDot();

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
    /// Sends an error message to the connected application.
    /// </summary>
    //void sendErrorToApp();

    /// <summary>
    /// Sends a rejection message to the connected application.
    /// </summary>
    // void sendRejection();

    /// <summary>
    /// Resends the last message if needed.
    /// </summary>
    void resendMessage();

    /// <summary>
    /// Checks if the connection to the Ball Spinner device is established.
    /// </summary>
    /// <returns>True if connected, false otherwise.</returns>
    bool isConnection();


}
