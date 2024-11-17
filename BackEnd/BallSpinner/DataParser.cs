using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;

/// <summary>
/// Class for distributing data from ball spinner to view/database/etc
/// </summary>
public class DataParser
{
    private event Action<Metric, float, float>? OnDataReceived;
    //public event SmartDotPacketRecieved;
    // Initialize writer. On a halt connection event, dispose method must be called.
    WriteToTempRevFile writer = new WriteToTempRevFile();
    /// <summary>
    /// Takes a parsed packet, and sends it to the rev file Writer and the Simulation
    /// </summary>
    /// <param name="packet"></param>
    public void SendSmartDotToSubscribers(string[] SmartDotData, Metric[] Metrics, float XData, float YData, float ZData, float timeStamp)
    { 
        // Send to writer
        writer.WriteData(SmartDotData);
        // Invoke the event for the simulation for each axis
        OnDataReceived?.Invoke(Metrics[0], XData, timeStamp); // Note: This is no longer timeFromStart, but a current timestamp
        OnDataReceived?.Invoke(Metrics[1], YData, timeStamp);
        OnDataReceived?.Invoke(Metrics[2], ZData, timeStamp);
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
