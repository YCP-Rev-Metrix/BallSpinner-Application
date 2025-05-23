﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using Common.POCOs;
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
    /// </summary>
    void ConnectSmartDot(PhysicalAddress? address);

    ///<summary>
    /// Tell the Ball Spinner to send available smart dots
    /// </summary>
    void ScanForSmartDots();

    ///<summary>
    /// Returns whether or not the MAC address for the associated SmartDot has been set.
    /// If it returns true, that means we have accepted a MAC address and are "paired" 
    /// </summary>
    bool IsSmartDotPaired();

    ///<summary>
    /// This updates the variables used to track the current SmartDot config.
    /// It also requests the TCP connection to send the chosen values to the BSC
    /// </summary>
    void SubmitSmartDotConfig(double[] Ranges, double[] SampleRates, bool XL_OFF, bool GY_OFF, bool MAG_OFF, bool LT_OFF);

    ///<summary>
    /// This returns an array of booleans that will be used to compare against the received Range settings
    /// values from B_A_RECEIVE_CONFIG_INFO and will be used to determine which options are visibile in the frontend.
    /// This will return an array of size 8
    /// </summary>
    List<List<double>> GetAvailableRanges();

    ///<summary>
    /// This returns an array of booleans that will be used to compare against the received sample rate settings
    /// values from B_A_RECEIVE_CONFIG_INFO and will be used to determine which options are visibile in the frontend.
    /// This will return an array of size 8
    List<List<double>> GetAvailableSampleRates();
    /// <summary>
    /// Represents the list of RPM values generated by the Bezier curve
    /// </summary>
    public List<double?> RPMList { get; set; }
    /// <summary>
    /// The number of RPMs within the RPMList
    /// </summary>
    public int RPMCount { get; set; }
    /// <summary>
    /// Used by the timer's elapsed event to keep track of the current RPM that needs to be sent to the ballspinner controller
    /// </summary>
    public int currentRPMInd { get; set; }
    /// <summary>
    /// Sets the inital values for the BallSpinner instance. This will be expanded to include things like ball and comments in the future.
    /// </summary>
    /// <param name="RPMs"></param>
    public void SetInitialValues(List<double?> RPMs, Coordinate BezierInit, Coordinate BezierInflection, Coordinate BezierFinal, string Comments, Ball ball);

    /// <summary>
    /// Indicates wheter or not the initial values are set for the 
    /// </summary>
    public bool InitialValuesSet => RPMList != null;

    public Coordinate BezierInitPoint { get; set; }

    public Coordinate BezierInflectionPoint { get; set; }

    public Coordinate BezierFinalPoint { get; set; }

    public Ball ball { get; set; }

    public string Comments { get; set; }
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
    Light = 1 << 9,

    MotorXFeedback = 1 << 10,

    MotorYFeedback = 1 << 11,

    MotorZFeedback = 1 << 12,
}