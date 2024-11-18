using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
    public DataParser DataParser { get; } = new DataParser();

    ///<inheritdoc/>
    public string Name { get; set; } = "Simulation";

    ///<inheritdoc/>
    public event Action? SendErrorToApp;

    ///<inheritdoc/>
    public event Action? SendRejection;

    ///<inheritdoc/>
    public event Action<bool>? OnConnectionChanged;

    ///<inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    private Timer? _timer;

    // New position and speed properties


    public Vector3 Position = Vector3.Zero; 
    
    public Vector3 velocity = Vector3.Zero;
    private Vector3 acceleration = new Vector3(0, 0, 1f);// Constant forward acceleration along Z

    public Vector3 AngularVelocity = new Vector3(0.1f,0.0f,1f);
    public Quaternion Rotation = Quaternion.Identity;

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
            DataParser.DataReceived(Metric.RotationX, eulerAngles.X, time);
            DataParser.DataReceived(Metric.RotationY, eulerAngles.Y, time);
            DataParser.DataReceived(Metric.RotationZ, eulerAngles.Z, time);

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
            DataParser.DataReceived(Metric.AccelerationX, acceleration.X, time);
            DataParser.DataReceived(Metric.AccelerationY, acceleration.Y, time);
            DataParser.DataReceived(Metric.AccelerationZ, acceleration.Z, time);

            // Send magnetometer data (where Z-Forward is north)
            Vector3 magnetometer = Vector3.TransformNormal(Vector3.UnitZ, Matrix4x4.CreateFromQuaternion(Rotation));
            DataParser.DataReceived(Metric.MagnetometerX, magnetometer.X, time);
            DataParser.DataReceived(Metric.MagnetometerY, magnetometer.Y, time);
            DataParser.DataReceived(Metric.MagnetometerZ, magnetometer.Z, time);

            // Send light data (where Y-Up is brightest)
            float light = Math.Max(Vector3.Dot(Vector3.TransformNormal(Vector3.UnitZ, Matrix4x4.CreateFromQuaternion(Rotation)), Vector3.UnitY), 0);
            DataParser.DataReceived(Metric.Light, light, time);
        }, null, frequency, frequency);
    }

    ///<inheritdoc/>
    public void Stop()
    {
        _timer?.Dispose();
    }

    private Vector3 GetEulerAngles(Quaternion rotation)
    {
        // Convert quaternion to euler angles (yaw, pitch, roll)
        float yaw = MathF.Atan2(2f * (rotation.Y * rotation.Z + rotation.W * rotation.X),
                                 rotation.W * rotation.W - rotation.X * rotation.X - rotation.Y * rotation.Y + rotation.Z * rotation.Z);
        float pitch = MathF.Asin(-2f * (rotation.X * rotation.Z - rotation.W * rotation.Y));
        float roll = MathF.Atan2(2f * (rotation.X * rotation.Y + rotation.W * rotation.Z),
                                  rotation.W * rotation.W + rotation.X * rotation.X - rotation.Y * rotation.Y - rotation.Z * rotation.Z);
        return new Vector3(yaw, pitch, roll);
    }
}
