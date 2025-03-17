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
    public string TempFilePath { get; private set; } = string.Empty;

    private event Action<Metric, float, float>? OnDataReceived;
    private WriteToTempRevFile? _writer;

    public void Start(string name)
    {
        TempFilePath = Utilities.GetTempRevFileDir(name);

        Stop();

        _writer = new WriteToTempRevFile(TempFilePath);
        _writer.Start();
    }

    public void Stop()
    {
        _writer?.Dispose();
        _writer = null;
    }

    /// <summary>
    /// Takes a parsed packet, and sends it to the rev file Writer and the Simulation
    /// </summary>
    public void SendSmartDotToSubscribers(SensorType sensorType, float deltaTime, int sampleCount, float XData, float YData, float ZData)
    {
        //Debug.WriteLine();
        string sensorTypeString;

        // Invoke the event for the simulation for each axis
        if (/*new Random().Next(10) > 5*/ true)
        {
            switch (sensorType)
            {
                case SensorType.Accelerometer:
                    OnDataReceived?.Invoke(Metric.AccelerationX, XData, deltaTime);
                    OnDataReceived?.Invoke(Metric.AccelerationY, YData, deltaTime);
                    OnDataReceived?.Invoke(Metric.AccelerationZ, ZData, deltaTime);
                    sensorTypeString = "3";
                    break;
                case SensorType.Gyroscope:
                    OnDataReceived?.Invoke(Metric.RotationX, XData, deltaTime);
                    OnDataReceived?.Invoke(Metric.RotationY, YData, deltaTime);
                    OnDataReceived?.Invoke(Metric.RotationZ, ZData, deltaTime);
                    sensorTypeString = "2";
                    break;
                case SensorType.Light:
                    OnDataReceived?.Invoke(Metric.Light, XData, deltaTime);
                    sensorTypeString = "1";
                    break;
                case SensorType.Magnetometer:
                    OnDataReceived?.Invoke(Metric.MagnetometerX, XData, deltaTime);
                    OnDataReceived?.Invoke(Metric.MagnetometerY, YData, deltaTime);
                    OnDataReceived?.Invoke(Metric.MagnetometerZ, ZData, deltaTime);
                    sensorTypeString = "4";
                    break;
                default:
                    sensorTypeString = "0";
                    break;
            }
            string[] smartDotData =
        {
            sensorTypeString, deltaTime.ToString(), sampleCount.ToString(), XData.ToString(), YData.ToString(), ZData.ToString()
        };

            _writer?.WriteData(smartDotData);
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
