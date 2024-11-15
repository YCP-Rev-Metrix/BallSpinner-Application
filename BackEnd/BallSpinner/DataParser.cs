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
    /// Takes a raw packet, deserializes it, and distributes the data
    /// </summary>
    /// <param name="packet"></param>
    public void ParsePacket(byte[] packet)
    {
        //distribute data to subscribers
        byte messageType = packet[0];
        // This indicates a SmartDot packet. Raise a SmartDotPacketRecieved event
        if (messageType == 0x8A)
        {
            int SensorType = packet[2];
            int SampleCount = packet[3] | (packet[4] << 8) | (packet[5] << 16);
            float timeStamp = packet[6] | (packet[7] << 8) | (packet[8] << 16) | (packet[9] << 32);
            float XData = packet[10] | (packet[11] << 8) | (packet[12] << 16) | (packet[13] << 32);
            float YData = packet[14] | (packet[15] << 8) | (packet[16] << 16) | (packet[17] << 32);
            float ZData = packet[18] | (packet[19] << 8) | (packet[20] << 16) | (packet[21] << 32);
            // Convert data to a string array
           string[] dataArray = new string[6] 
           {
               SensorType.ToString(), SampleCount.ToString(), timeStamp.ToString(), XData.ToString(), YData.ToString(), ZData.ToString()
           };

            // Send to writer
            writer.WriteData(dataArray);
            // Define the metric type
            Metric TypeX = (Metric) packet[2];
            Metric TypeY = (Metric) (packet[2] << 1);
            Metric TypeZ = (Metric) (packet[2] << 2);
            // Invoke the event for the simulation for each axis
            OnDataReceived?.Invoke(TypeX, XData, timeStamp); // Note: This is no longer timeFromStart, but a current timestamp
            OnDataReceived?.Invoke(TypeY, YData, timeStamp);
            OnDataReceived?.Invoke(TypeZ, ZData, timeStamp);
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
