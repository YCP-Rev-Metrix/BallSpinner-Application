using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;

/// <summary>
/// Class for distributing data from ball spinner to view/database/etc
/// </summary>
public class DataParser
{
    private event Action<Metric, float, float>? OnDataReceived;
    //public event SmartDotPacketRecieved;
    // Initialize writer. On a halt connection event, dispose method must be called.
    //WriteToTempRevFile writer = new WriteToTempRevFile(Utilities.GetTempDir()+"/TempRev.csv");
    /// <summary>
    /// Takes a parsed packet, and sends it to the rev file Writer and the Simulation
    /// </summary>
    public void SendSmartDotToSubscribers(SensorType sensorType, float timeStamp, int sampleCount, float XData, float YData, float ZData)
    {
        // Send to writer
        //writer.WriteData(SmartDotData);
        //Debug.WriteLine();

        // Invoke the event for the simulation for each axis
        switch (sensorType)
        {
            case SensorType.Accelerometer:
                OnDataReceived?.Invoke(Metric.AccelerationX, XData, timeStamp);
                OnDataReceived?.Invoke(Metric.AccelerationY, YData, timeStamp);
                OnDataReceived?.Invoke(Metric.AccelerationZ, ZData, timeStamp);

                break;
            case SensorType.Gyroscope:
                OnDataReceived?.Invoke(Metric.RotationX, XData, timeStamp);
                OnDataReceived?.Invoke(Metric.RotationY, YData, timeStamp);
                OnDataReceived?.Invoke(Metric.RotationZ, ZData, timeStamp);

                break;
            case SensorType.Light:
                OnDataReceived?.Invoke(Metric.Light, XData, timeStamp);

                break;
            case SensorType.Magnetometer:
                OnDataReceived?.Invoke(Metric.MagnetometerX, XData, timeStamp);
                OnDataReceived?.Invoke(Metric.MagnetometerY, YData, timeStamp);
                OnDataReceived?.Invoke(Metric.MagnetometerZ, ZData, timeStamp);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Distributes a data point to all subscribers
    /// </summary>
    public void DataReceived(Metric metric, float value, float timeFromStart)
    {
        OnDataReceived?.Invoke(metric, value, timeFromStart);
    }

    /// <summary>
    /// Start listening to data. The method provided will be invoked
    /// </summary>
    public void Subscribe(Action<Metric, float, float> onDataReceived)
    {
        OnDataReceived += onDataReceived;
    }

    /// <summary>
    /// Stop listening to data.
    /// </summary>
    /// <param name="onDataReceived"></param>
    public void Unsubscribe(Action<Metric, float, float> onDataReceived)
    {
        OnDataReceived -= onDataReceived;
    }
}
