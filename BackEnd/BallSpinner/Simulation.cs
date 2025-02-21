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

    ///<inheritdoc/>
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
        Stop();

        DataParser.Start(Name);

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
    public void SubmitSmartDotConfig(double[] ODRs, double[] SampleRates)
    {
        if (!IsSmartDotPaired())
            return;
        //tcp function for sending config message

    }
    /// <inheritdoc/>
    public int[] GetAvailableRanges()
    {
        throw new NotImplementedException();
    }

    /// <inheritidoc/>
    public int[] GetAvailableSampleRates()
    {
        throw new NotImplementedException();
    }
    public async void ScanForSmartDots()
    {
        throw new NotImplementedException();
    }

}
