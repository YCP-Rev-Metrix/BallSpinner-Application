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
    
    // Define the delegate for the event handler
    public delegate void OnDataRecievedEvent(byte[] packet);
    
    // Declare the event based on the delegate
    public event OnDataRecievedEvent OnDataRecieved;

    /// <summary/>
    public TCP(IPAddress address)
    {
        _client = new TcpClient();
        _address = address;
        Task.Run(Connect);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client.Dispose();
    }

    private async Task Connect()
    {
        try
        {
            //await _client.ConnectAsync(_address, PORT);
            byte token = (byte)new Random().Next(255);

            //Handshake by telling BallSpinner that I want to connect
            _send[0] = (byte)MessageType.Connect;
            _send[1] = 1; //No data to send
            _send[2] = token; //Random token, BallSpinner MUST send back the same value

            int sent = await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 3));
            if (sent <= 0)
                throw new Exception("Handshake request could not be sent");

            //Receive ball spinner data, if data is not receieved the connection was rejected
            int receieved = await _client.Client.ReceiveAsync(_receive);
            if (receieved <= 0)
                throw new Exception("Handshake request could not be received");

            if (_receive[0] != (byte)MessageType.ConnectResponse)
                throw new Exception("Host sent wrong data! Handshake failed.");

            if (_receive[1] != 1)
                throw new Exception("Host sent too much data! Handshake failed.");

            if (_receive[2] != token)
                throw new Exception("Host did not respond with same token! Handshake failed.");

            //02 00 FF
            byte messageType = _receive[0];
            int messageSize = _receive[1] + (_receive[2] << sizeof(byte));

            if (_client.Connected)
                Listen();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async void Listen()
    {
        while (_client.Connected)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int size = await _client.Client.ReceiveAsync(buffer);

            if (size <= 0)
                return;

            //Format
            //Byte 0 = Message type
            //Byte 1-2 = Message size (Maximum 65,536 bytes)
            //

            byte messageType = buffer[0];
            int messageSize = buffer[1] + (buffer[2] << sizeof(byte));

            if (size > 0)
                Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, size));
                OnDataRecievedEventHandler(buffer);
        }
    }

    protected virtual void OnDataRecievedEventHandler(byte[] packet)
    {
        OnDataRecieved?.Invoke(packet);
    }
}

/// <summary>
/// Types of messages that can be sent to the ball spinner
/// </summary>
public enum MessageType : byte
{
    /// <summary>
    /// Connect to the ball spinner
    /// </summary>
    Connect = 0x01,

    /// <summary>
    /// Connect to the ball spinner
    /// </summary>
    ConnectResponse = 0x02,

    /// <summary>
    /// Send raw text
    /// </summary>
    Text =  0xFF,
}