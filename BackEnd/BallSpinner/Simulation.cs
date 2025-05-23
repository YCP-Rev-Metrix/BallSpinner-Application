﻿using Common.POCOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;
/// <summary>
/// Interface defining the structure for simulation components in the backend.
/// </summary>
public class Simulation : IBallSpinner
{
    ///<inheritdoc/>
    public string MAC { get; set; } = "Simulation";

    ///<inheritdoc/>
    public DataParser DataParser { get; private set; } = new DataParser();

    ///<inheritdoc/>
    public string Name { get; set; } = "Simulation";

    public string SmartDotMAC { get; } = "11:11:11:11:11:11";

    ///<inheritdoc/>
    public event Action? SendErrorToApp;

    ///<inheritdoc/>
    public event Action? SendRejection;

    ///<inheritdoc/>
    public event Action<bool>? OnConnectionChanged;

    ///<inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    ///<inheritdoc/>
    public event Action<PhysicalAddress> OnSmartDotMACAddressReceived;

    private Timer? _timer;

    public List<double> accelXValues = new List<double>();
    public List<double> accelYValues = new List<double>();
    public List<double> accelZValues = new List<double>();
    public List<double> rotatXValues = new List<double>();
    public List<double> rotatYValues = new List<double>();
    public List<double> rotatZValues = new List<double>();
    public List<double> magneXValues = new List<double>();
    public List<double> magneYValues = new List<double>();
    public List<double> magneZValues = new List<double>();
    public List<double> lightValues = new List<double>();

    // New position and speed properties


    public Vector3 Position = Vector3.Zero; 
    
    public Vector3 velocity = Vector3.Zero;
    private Vector3 acceleration = new Vector3(0, 0, 1f);// Constant forward acceleration along Z

    public Vector3 AngularVelocity = new Vector3(0.1f,0.0f,1f);
    public Quaternion Rotation = Quaternion.Identity;

    ///<inheritdoc/>
    public List<double?> RPMList { get; set; } = null;
    ///<inheritdoc/>
    public int RPMCount { get; set; } = 0;
    ///<inheritdoc/>
    public int currentRPMInd { get; set; } = 0;
    ///<inheritdoc/>
    public bool InitialValuesSet => RPMList != null && BezierInitPoint != null && BezierInflectionPoint != null && BezierFinalPoint != null && ball != null && Comments != null;

    public Coordinate BezierInitPoint { get; set; }

    public Coordinate BezierInflectionPoint { get; set; }

    public Coordinate BezierFinalPoint { get; set; }

    public Ball ball { get; set; }

    public string Comments { get; set; }

    private IEnumerator<double?> enumerator;

    /// <summary/>
    public Simulation()
    {
        InitializeConnection();
    }

    /// <summary>
    /// 
    /// Disposes of resources used by the simulation component.
    /// </summary>
    public void Dispose()
    {
        _timer?.Dispose();
    }

    ///<inheritdoc/>
    public void InitializeConnection()
    {
        OnConnectionChanged?.Invoke(true);
    }

    ///<inheritdoc/>
    public bool IsConnected() => true;

    ///<inheritdoc/>
    public void ResendMessage()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public List<string> SendBackListOfSmartDots()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public void SetSmartDot()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public void Start()
    {
        Stop();

        // Get enumerator for RPM values
        enumerator = RPMList.GetEnumerator();

        bool Completed = false;

        DataParser.Start(Name);
        // For now timeStep is hardcoded, but when this is no longer true, the TimeSpan.FromSeconds needs to be altered to be variable
        TimeSpan frequency = TimeSpan.FromSeconds(1 / 10f);
        _timer = new Timer((o) =>
        {

            Rotation *= Quaternion.CreateFromYawPitchRoll(
                AngularVelocity.Y * (float)frequency.TotalSeconds, // Yaw
                AngularVelocity.X * (float)frequency.TotalSeconds, // Pitch
                AngularVelocity.Z * (float)frequency.TotalSeconds  // Roll
            );

            Vector3 eulerAngles = GetEulerAngles(Rotation);

            // Update time and calculate rotation data
            float time = (float)DateTime.UtcNow.TimeOfDay.TotalSeconds;
            DataParser.SendSmartDotToSubscribers(SensorType.Gyroscope, time, 0, eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

            // Update velocity based on constant acceleration
            velocity += acceleration * (float)frequency.TotalSeconds;

            // Update position based on current velocity and time elapsed
            Position += velocity * (float)frequency.TotalSeconds;

            // Log velocity and acceleration to the console
            /*Debug.WriteLine($"Time: {time}");
            Debug.WriteLine($"Position: X={Position.X}, Y={Position.Y}, Z={Position.Z}");
            Debug.WriteLine($"Velocity: X={velocity.X}, Y={velocity.Y}, Z={velocity.Z}");
            Debug.WriteLine($"Acceleration: X={acceleration.X}, Y={acceleration.Y}, Z={acceleration.Z}");*/

            // Send updated velocity data as Acceleration metrics
            DataParser.SendSmartDotToSubscribers(SensorType.Accelerometer, time, 0, acceleration.X, acceleration.Y, acceleration.Z);

            // Send magnetometer data (where Z-Forward is north)
            Vector3 magnetometer = Vector3.TransformNormal(Vector3.UnitZ, Matrix4x4.CreateFromQuaternion(Rotation));
            DataParser.SendSmartDotToSubscribers(SensorType.Magnetometer, time, 0, magnetometer.X, magnetometer.Y, magnetometer.Z);

            // Send light data (where Y-Up is brightest)
            float light = Math.Max(Vector3.Dot(Vector3.TransformNormal(Vector3.UnitZ, Matrix4x4.CreateFromQuaternion(Rotation)), Vector3.UnitY), 0);
            DataParser.SendSmartDotToSubscribers(SensorType.Light, time, 0, light, 0, 0);


            if (!Completed)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        Console.WriteLine("End of collection reached.");
                        // Ensure enumerator does not continue
                        Completed = true;
                        break;
                    }
                }
            }
            // Send RPM value
            if (!Completed)
            {
                DataParser.SendSmartDotToSubscribers(SensorType.MotorXFeedback, time, 0, (float)enumerator.Current, 0, 0);
                enumerator.MoveNext();
            }
            UpdateCharts();

        }, null, frequency, frequency);
    }

    ///<inheritdoc/>
    public void Stop()
    {
        _timer?.Dispose();
        DataParser.Stop();
    }

    private Vector3 GetEulerAngles(Quaternion rotation)
    {
        // Convert quaternion to euler angles (yaw, pitch, roll)
        float yaw = MathF.Atan2(2f * (rotation.Y * rotation.Z + rotation.W * rotation.X),
                                 rotation.W * rotation.W - rotation.X * rotation.X - rotation.Y * rotation.Y + rotation.Z * rotation.Z);
        float pitch = MathF.Asin(-2f * (rotation.X * rotation.Z - rotation.W * rotation.Y));
        float roll = MathF.Atan2(2f * (rotation.X * rotation.Y + rotation.W * rotation.Z),
                                  rotation.W * rotation.W + rotation.X * rotation.X - rotation.Y * rotation.Y - rotation.Z * rotation.Z);
        return new Vector3(yaw, pitch, roll) / ((float)Math.PI / 180);
    }

    /// <inheritdoc/>
    public void ConnectSmartDot(PhysicalAddress? address)
    {
        if (address == null)
            OnSmartDotMACAddressReceived?.Invoke(PhysicalAddress.Parse(SmartDotMAC));
    }
    /// <inheritdoc/>
    public bool IsSmartDotPaired()
    {
        if (!IsConnected())
            return false;

        if (!string.IsNullOrEmpty(SmartDotMAC))
            return false;

        return true;
    }

    /// <inheritdoc/>
    public void SubmitSmartDotConfig(double[] ODRs, double[] SampleRates, bool XL_OFF, bool GY_OFF, bool MAG_OFF, bool LT_OFF)
    {
        if (!IsSmartDotPaired())
            return;
        //tcp function for sending config message

    }
    /// <inheritdoc/>
    public List<List<double>> GetAvailableRanges()
    {
        throw new NotImplementedException();
    }

    /// <inheritidoc/>
    public List<List<double>> GetAvailableSampleRates()
    {
        throw new NotImplementedException();
    }
    public async void ScanForSmartDots()
    {
        throw new NotImplementedException();
    }

    private void UpdateCharts()
    {
        accelXValues.Add((double)Metric.AccelerationX);
        accelYValues.Add((double)Metric.AccelerationY);
        accelZValues.Add((double)Metric.AccelerationZ);
        rotatXValues.Add((double)Metric.RotationX);
        rotatYValues.Add((double)Metric.RotationY);
        rotatZValues.Add((double)Metric.RotationZ);
        magneXValues.Add((double)Metric.MagnetometerX);
        magneYValues.Add((double)Metric.MagnetometerY);
        magneZValues.Add((double)Metric.MagnetometerZ);
        lightValues.Add((double)Metric.Light);
    }

    /// <inheritidoc/>
    public void SetInitialValues(List<double?> RPMs, Coordinate BezierInit, Coordinate BezierInflection, Coordinate BezierFinal, string Comments, Ball ball)
    {
        // Set RPMs for motor instructions
        this.RPMList = RPMs;
        this.RPMCount = RPMs.Count;
        this.BezierInitPoint = BezierInit;
        this.BezierInflectionPoint = BezierInflection;
        this.BezierFinalPoint = BezierFinal;
        this.Comments = Comments;
        this.ball = ball;
        PropertyChanged.Invoke(null, new PropertyChangedEventArgs("InitialValuesSet"));
    }

}
