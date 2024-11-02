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

    /// <summary>
    /// Takes a raw packet, deserializes it, and distributes the data
    /// </summary>
    /// <param name="packet"></param>
    public void ParsePacket(byte[] packet)
    {
        //distribute data to subscribers
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
