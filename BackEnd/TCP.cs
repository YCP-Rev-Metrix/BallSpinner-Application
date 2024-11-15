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

    private TcpClient _client;
    private IPAddress _address;

    private const int BUFFER_SIZE = 1024;
    private const ushort PORT = 8411;

    private readonly byte[] _receive = new byte[BUFFER_SIZE];
    private readonly byte[] _send = new byte[BUFFER_SIZE];

    private Dictionary<MessageType, TaskCompletionSource<object>> _pendingMessages = new Dictionary<MessageType, TaskCompletionSource<object>>();

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

    public async Task<string> GetDeviceInfo()
    {
        var task = new TaskCompletionSource<object>();
        _pendingMessages.Add(MessageType.GetDeviceInfo, task);

        _send[0] = (byte)MessageType.GetDeviceInfo;
        _send[1] = 0;
        _send[2] = 0;
        await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 3));

        var result = await task.Task;
        return (string)result;
    }

    public async Task Connect()
    {
        try
        {
            await _client.ConnectAsync(_address, PORT);
            byte token = (byte)new Random().Next(255);

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

            Task.Run(Listen);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async void Listen()
    {
        while (true)
        {
            int size = await _client.Client.ReceiveAsync(_receive);

            if (size <= 0)
                return;

            //Format
            //Byte 0 = Message type
            //Byte 1-2 = Message size (Maximum 65,536 bytes)
            byte messageTypeByte = _receive[0];
            MessageType messageType = (MessageType)Enum.ToObject(typeof(MessageType), messageTypeByte);

            switch (messageType)
            {
                case MessageType.GetDeviceInfo:
                case MessageType.GetDeviceInfoResponse:
                    int messageSize = _receive[1] + (_receive[2] << sizeof(byte));

                    string text = Encoding.UTF8.GetString(_receive, 0, size);
                    Debug.WriteLine(text);

                    if(_pendingMessages.Remove(MessageType.GetDeviceInfo, out var task))
                        task.SetResult(text);

                    break;
                default:
                    throw new Exception($"Invalid message type: {messageTypeByte}");
            }
        }
    }
}

/// <summary>
/// Types of messages that can be sent to the ball spinner
/// 2 bits = version number
/// 6 bits = message
/// </summary>
public enum MessageType : byte
{
    /// <summary>
    /// Connect to the ball spinner
    /// </summary>
    Connect = 0x81,
    /// <summary>
    /// Response from server about connecting to the ball spinner
    /// </summary>
    ConnectResponse = 0x82,

    /// <summary>
    /// Request for device information
    /// </summary>
    GetDeviceInfo = 0x83,

    /// <summary>
    /// Response from server containing device information
    /// </summary>
    GetDeviceInfoResponse = 0x84,

    /// <summary>
    /// Send raw text
    /// </summary>
    Text =  0xFF,
}