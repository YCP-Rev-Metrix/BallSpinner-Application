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
public class DataParser : IDisposable
{
    public string TempFilePath { get; private set; } = string.Empty;

    private event Action<Metric, float, float>? OnDataReceived;
    private WriteToTempRevFile? _writer;

    public int NumRecords { get; set; } = 0;
    /// <summary>
    /// Represents the number of data points within the sensor packets. Whenever the sensor packets are altered, this constant should 
    /// change along with it.
    /// </summary>
    public static int NUM_DATA_POINTS { get; } = 6;
    public void Start(string name)
    {
        Stop();

        TempFilePath = name;

        _writer = new WriteToTempRevFile(name);
        _writer.OnRecordAdded += HandleRecordAdded;
        _writer.Start();
    }
    /// <summary>
    /// Releases all unneeded resources.
    /// </summary>
    public void Stop()
    {
        // For now, these are commented out because dispose of _writer deletes memory mapped file
        //_writer?.Dispose();
        //_writer = null;
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
                default:
                    sensorTypeString = "0";
                    break;
            }
            string[] smartDotData =
        {
            sensorTypeString, timeStamp.ToString(), sampleCount.ToString(), XData.ToString(), YData.ToString(), ZData.ToString()
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

    public void Dispose()
    {
        _writer?.Dispose();
    }

    private void HandleRecordAdded()
    {
        NumRecords++;
    }
}
