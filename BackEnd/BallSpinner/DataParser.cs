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
public class DataParser: IDisposable
{
    public string TempFilePath { get; private set; } = string.Empty;

    private event Action<Metric, float, float>? OnDataReceived;
    public event Action<double, bool> OnDataParserStart;
    private WriteToTempRevFile? _writer;

    public void Start(string name)
    {
        TempFilePath = Utilities.GetTempRevFileDir(name);

        Stop();

        _writer = new WriteToTempRevFile(TempFilePath);
        _writer.Start();

        OnDataParserStart.Invoke(0.01, true);
    }

    public void Stop()
    {
        _writer?.Dispose();
        _writer = null;
        OnDataParserStart.Invoke(0.01, false);
    }

    /// <summary>
    /// Takes a parsed packet, and sends it to the rev file Writer and the Simulation
    /// </summary>
    public void SendSmartDotToSubscribers(SensorType sensorType, float timeStamp, int sampleCount, float XData, float YData, float ZData)
    {
        //Debug.WriteLine();
        string sensorTypeString;

        // Invoke the event for the simulation for each axis
        if (/*new Random().Next(10) > 5*/ true)
        {
            switch (sensorType)
            {
                case SensorType.Accelerometer:
                    OnDataReceived?.Invoke(Metric.AccelerationX, XData, timeStamp);
                    OnDataReceived?.Invoke(Metric.AccelerationY, YData, timeStamp);
                    OnDataReceived?.Invoke(Metric.AccelerationZ, ZData, timeStamp);
                    sensorTypeString = "3";
                    break;
                case SensorType.Gyroscope:
                    OnDataReceived?.Invoke(Metric.RotationX, XData, timeStamp);
                    OnDataReceived?.Invoke(Metric.RotationY, YData, timeStamp);
                    OnDataReceived?.Invoke(Metric.RotationZ, ZData, timeStamp);
                    sensorTypeString = "2";
                    break;
                case SensorType.Light:
                    OnDataReceived?.Invoke(Metric.Light, XData, timeStamp);
                    sensorTypeString = "1";
                    break;
                case SensorType.Magnetometer:
                    OnDataReceived?.Invoke(Metric.MagnetometerX, XData, timeStamp);
                    OnDataReceived?.Invoke(Metric.MagnetometerY, YData, timeStamp);
                    OnDataReceived?.Invoke(Metric.MagnetometerZ, ZData, timeStamp);
                    sensorTypeString = "4";
                    break;
                case SensorType.MotorXFeedback: // primary motor
                    OnDataReceived?.Invoke(Metric.MotorXFeedback, XData, timeStamp);
                    Debug.WriteLine("Encoder Value Recieved: " + XData);
                    sensorTypeString = "5";
                    break;
                default:
                    sensorTypeString = null;
                    break;
            }
            string[] smartDotData =
        {
            sensorTypeString, timeStamp.ToString(), sampleCount.ToString(), XData.ToString(), YData.ToString(), ZData.ToString()
        };
            // For now we are recieving sensors that cannot be stored in the database, so nothing should be written to cache file for these
            if (sensorTypeString != null)
            {
                _writer?.WriteData(smartDotData);

            }
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

    public void Dispose()
    {
        _writer?.Dispose();
    }
    public void StopBallRotation()
    {
        OnDataParserStart.Invoke(0.01, false);
    }
}
