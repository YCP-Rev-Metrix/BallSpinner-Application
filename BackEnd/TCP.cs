using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using static System.Net.Mime.MediaTypeNames;

namespace RevMetrix.BallSpinner.BackEnd;

/// <summary>
/// TCP client that connects to the ball spinner device
/// </summary>
public class TCP : IDisposable
{
    /// <summary>
    /// True if there is an existing TCP connection
    /// </summary>
    public bool Connected => _client.Connected;

    public delegate void SmartDotRecievedHandler(string[] SmartDotData, Metric[] Metrics, float XData, float YData, float ZData, float timeStamp);

    /// <summary>
    /// Fired when a SmartDot packet is recieved
    /// </summary>
    public event SmartDotRecievedHandler? SmartDotRecievedEvent;
    
    private TcpClient _client;
    private IPAddress _address;

    private const int BUFFER_SIZE = 1024;
    private const ushort PORT = 8411;

    private readonly byte[] _receive = new byte[BUFFER_SIZE];
    private readonly byte[] _send = new byte[BUFFER_SIZE];

    private List<(MessageType Type, TaskCompletionSource<object> Task)> _pendingMessages = new();
    
    /// <summary/>
    public TCP(IPAddress address)
    {
        _client = new TcpClient();
        _address = address;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client.Dispose();
    }

    /// <summary>
    /// Gets the name of the connected device
    /// </summary>
    public async Task<string> GetDeviceInfo()
    {
        var task = new TaskCompletionSource<object>();
        _pendingMessages.Add((MessageType.GetDeviceInfoResponse, task));

        _send[0] = (byte)MessageType.GetDeviceInfo;
        _send[1] = 0;
        _send[2] = 0;
        await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 3));

        var result = await task.Task;
        return (string)result;
    }


    /// <summary>
    /// Gets the name of the connected device
    /// </summary>
    public async Task<PhysicalAddress> ConnectSmartDot()
    {
        var task = new TaskCompletionSource<object>();
        _pendingMessages.Add((MessageType.ConnectSmartDotResponse, task));

        _send[0] = (byte)MessageType.ConnectSmartDot;
        _send[1] = 0;
        _send[2] = 0;
        await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 3));

        var result = await task.Task;
        return (PhysicalAddress)result;
    }

    /// <summary>
    /// Initialize the TCP connection
    /// </summary>
    public async Task Connect()
    {
        try
        {
            await _client.ConnectAsync(_address, PORT);
            byte token = (byte)new Random().Next(byte.MaxValue);

            //Handshake by telling BallSpinner that I want to connect
            _send[0] = (byte)MessageType.Connect;
            _send[2] = 1; //No data to send
            _send[3] = token; //Random token, BallSpinner MUST send back the same value

            int sent = await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 4));
            if (sent <= 0)
                throw new Exception("Handshake request could not be sent");

            //Receive ball spinner data, if data is not receieved the connection was rejected
            int receieved = await _client.Client.ReceiveAsync(_receive);
            if (receieved <= 0)
                throw new Exception("Handshake request could not be received");

            if (_receive[0] != (byte)MessageType.ConnectResponse)
                throw new Exception("Host sent wrong data! Handshake failed.");

            if (_receive[2] != 1)
                throw new Exception("Host sent too much data! Handshake failed.");

            if (_receive[3] != token)
                throw new Exception("Host did not respond with same token! Handshake failed.");

            Debug.WriteLine("Connected :)");

            var _ = Task.Run(Listen); //Do not await the task
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private (MessageType Type, int Size) GetMessageInfo(byte[] packet)
    {
        //Format
        //Byte 0 = Message type
        //Byte 1-2 = Message size (Maximum 65,536 bytes)
        byte messageTypeByte = _receive[0];
        MessageType messageType = (MessageType)Enum.ToObject(typeof(MessageType), messageTypeByte);
        int messageSize = _receive[2] + (_receive[1] << 8);

        return (messageType, messageSize);
    }

    private async void Listen()
    {
        while (true)
        {
            int size = await _client.Client.ReceiveAsync(_receive);

            if (size <= 0)
                return;

            var (messageType, messageSize) = GetMessageInfo(_receive);

            switch (messageType)
            {
                case MessageType.GetDeviceInfo:
                case MessageType.GetDeviceInfoResponse:

                    string text = Encoding.UTF8.GetString(_receive, 3, messageSize);
                    Debug.WriteLine(text);

                    var task = TryGetResponse(messageType);
                    task!.SetResult(text);

                    break;

                case MessageType.ConnectSmartDot:
                case MessageType.ConnectSmartDotResponse:
                    var physicalAddress = new PhysicalAddress(new ArraySegment<byte>(_receive, 3, messageSize).ToArray());
                    var p = physicalAddress.ToString();
                    var task2 = TryGetResponse(messageType);
                    task2!.SetResult(physicalAddress);

                    break;
                case MessageType.SmartDotDataPacket:
                    //distribute data to subscribers
                    int SensorType = _receive[2];
                    int SampleCount = _receive[3] | (_receive[4] << 8) | (_receive[5] << 16);
                    float timeStamp = _receive[6] | (_receive[7] << 8) | (_receive[8] << 16) | (_receive[9] << 32);
                    float XData = _receive[10] | (_receive[11] << 8) | (_receive[12] << 16) | (_receive[13] << 32);
                    float YData = _receive[14] | (_receive[15] << 8) | (_receive[16] << 16) | (_receive[17] << 32);
                    float ZData = _receive[18] | (_receive[19] << 8) | (_receive[20] << 16) | (_receive[21] << 32);
                    // Convert data to a string array
                    string[] SmartDotData = new string[6] 
                    {
                        SensorType.ToString(), SampleCount.ToString(), timeStamp.ToString(), XData.ToString(), YData.ToString(), ZData.ToString()
                    }; 
                    // Define the metric type's
                    Metric TypeX = (Metric) _receive[2];
                    Metric TypeY = (Metric) (_receive[2] << 1);
                    Metric TypeZ = (Metric) (_receive[2] << 2);
                    Metric[] CurrentMetricTypes = {TypeX, TypeY, TypeZ};
                    SmartDotRecievedEvent?.Invoke(SmartDotData, CurrentMetricTypes, XData, YData, ZData, timeStamp);
                    break;
                default:
                    throw new Exception($"Unexpected message type '{messageType}'");
            }
        }
    }

    private TaskCompletionSource<object>? TryGetResponse(MessageType messageType)
    {
        for (int i = 0; i < _pendingMessages.Count; i++)
        {
            var message = _pendingMessages[i];
            if (message.Type == messageType)
            {
                _pendingMessages.RemoveAt(i);
                return message.Task;
            }
        }

        return null;
    }
    /// <summary>
    /// For now unimplemented. Will be used to send motor instruction packets to SmartDot
    /// </summary>
    public async void SendPacketToSmartDot(byte[] instructions)
    {
        if (!Connected)
        {
            throw new Exception("Must establish a connection to the ballspinner first");
        }
        else 
        {
            throw new NotImplementedException();
        }
    }
}

/// <summary>
/// Types of messages that can be sent to the ball spinner
/// First 2 bits = Version number
/// Last 6 bits = Message Type
/// </summary>
public enum MessageType : byte
{
    /// <summary>
    /// Connect to the ball spinner
    /// </summary>
    Connect = 0b10_00_00_01,

    /// <summary>
    /// Response from server about connecting to the ball spinner
    /// </summary>
    ConnectResponse = 0b10_00_00_10,

    /// <summary>
    /// Request for device information
    /// </summary>
    GetDeviceInfo = 0b10_00_00_11,

    /// <summary>
    /// Response from server containing device information
    /// </summary>
    GetDeviceInfoResponse = 0b10_00_01_00,

    /// <summary>
    /// Tell the server to connect to the smart dot module
    /// </summary>
    ConnectSmartDot = 0x85,

    /// <summary>
    /// Response from server containing smart dot connection info
    /// </summary>
    ConnectSmartDotResponse = 0x86,
    
    /// <summary>
    /// Indicates a SmartDot data packet
    /// </summary>
    SmartDotDataPacket = 0x8A,

    /// <summary>
    /// Send or receive raw, UTF-8 text
    /// </summary>
    Text =  0b11_11_11_11,
}