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
    /// <summary>
    /// Method signature for the SmartDotReceived event 
    /// </summary>
    public delegate void SmartDotRecievedHandler(SensorType sensorType, float timeStamp, int sampleCount, float XData, float YData, float ZData);

    /// <summary>
    /// Fired when a SmartDot packet is recieved
    /// </summary>
    public event SmartDotRecievedHandler? SmartDotReceivedEvent;

    /// <summary>
    /// Invoked when a Smart dot address is received from the device
    /// </summary>
    public event Action<PhysicalAddress> SmartDotAddressReceivedEvent;
    
    private TcpClient _client;
    private IPAddress _address;

    private const int BUFFER_SIZE = 16384;
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
    /// Connect to a smart dot module. Sending null indicates you want to start listening to packets
    /// </summary>
    public async Task ConnectSmartDot(PhysicalAddress? address)
    {
        _send[0] = (byte)MessageType.ConnectSmartDot;
        _send[1] = 0;
        _send[2] = 6;
        var bytes = address?.GetAddressBytes();
        _send[3] = bytes != null ? bytes[0] : (byte)0;
        _send[4] = bytes != null ? bytes[1] : (byte)0;
        _send[5] = bytes != null ? bytes[2] : (byte)0;
        _send[6] = bytes != null ? bytes[3] : (byte)0;
        _send[7] = bytes != null ? bytes[4] : (byte)0;
        _send[8] = bytes != null ? bytes[5] : (byte)0;
        await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 9));
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

    private (MessageType Type, int Size) GetMessageInfo(ArraySegment<byte> packet)
    {
        //Format
        //Byte 0 = Message type
        //Byte 1-2 = Message size (Maximum 65,536 bytes)
        byte messageTypeByte = packet[0];
        MessageType messageType = (MessageType)Enum.ToObject(typeof(MessageType), messageTypeByte);
        int messageSize = packet[2] + (packet[1] << 8);

        return (messageType, messageSize);
    }

    private async void Listen()
    {
        while (true)
        {
            int size = 0;

            try
            {
                size = await _client.Client.ReceiveAsync(_receive);

                if (size == 0)
                    continue;

                int currentIndex = 0;
                while (currentIndex < size && _receive[currentIndex] != 0)
                {
                    var packetFixed = new byte[3 + _receive[currentIndex + 2] + (_receive[currentIndex + 1] << 8) + 1];

                    for (int i = 0; i < 3 + _receive[currentIndex + 2] + (_receive[currentIndex + 1] << 8); i++)
                    {
                        packetFixed[i] = _receive[currentIndex + i];
                    }

                    currentIndex += packetFixed.Length;

                    var (messageType, messageSize) = GetMessageInfo(packetFixed);
                    _receive[currentIndex] = 0;

                    switch (messageType)
                    {
                        case MessageType.GetDeviceInfo:
                        case MessageType.GetDeviceInfoResponse:

                            string text = Encoding.UTF8.GetString(packetFixed, 3, messageSize);
                            Debug.WriteLine(text);

                            var task = TryGetResponse(messageType);
                            task?.SetResult(text);

                            break;

                        case MessageType.ConnectSmartDot:
                        case MessageType.ConnectSmartDotResponse:
                            var physicalAddress = new PhysicalAddress(new ArraySegment<byte>(packetFixed, 3, messageSize).ToArray());
                            SmartDotAddressReceivedEvent?.Invoke(physicalAddress);

                            break;
                        case MessageType.SmartDotDataPacket:
                            SensorType sensorType = (SensorType)Enum.ToObject(typeof(SensorType), packetFixed[3]);
                            int sampleCount = packetFixed[6] | (packetFixed[5] << 8) | (packetFixed[4] << 16); //3 bytes
                            float timeStamp = BitConverter.ToSingle(packetFixed, 7); //BitConverter expects LITTLE ENDIAN
                            float xData = BitConverter.ToSingle(packetFixed, 11);
                            float yData = BitConverter.ToSingle(packetFixed, 15);
                            float zData = BitConverter.ToSingle(packetFixed, 19);
                            
                            Debug.WriteLine($"{sensorType}: {xData} {yData} {zData}");
                            SmartDotReceivedEvent?.Invoke(sensorType, timeStamp, sampleCount, xData, yData, zData);
                            break;
                        default:
                            break;
                            //throw new Exception($"Unexpected message type '{messageType}'");
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                //_client.Dispose();
                return;
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
    /// Sends voltages to the motors
    /// </summary>
    public async void SetMotorVoltages(byte x, byte y, byte z)
    {
        if (!_client.Connected)
            throw new Exception("Can't send instructions without being connected");

        // FOR NOW! This is a script that will send automated instructions for MS2
        // This needs to be refactored to send predefined instructions based on kinematic calculations
        byte[] instructions = new byte[]
        {
            0x88, //Type

            0x00,
            0x03, //Size

            x, //Motor x
            y, //Motor y
            z, //Motor z
        };

        // Send the motor instruction to the PI
        await _client.Client.SendAsync(instructions);
    }

    /// <summary>
    /// Stops sending instructions to the motor
    /// </summary>
    public async void StopMotorInstructions()
    {
        if (!_client.Connected)
            throw new Exception("Can't send instructions without being connected");

        //Sends a STOP_MOTOR_INSTUCTIONS per Roberts Protocol sheet
        byte[] instructions = new byte[]
        {
            0x0B, //Type
        };

        // Send the motor instruction to the PI
        await _client.Client.SendAsync(instructions);
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

public enum SensorType : byte
{ 
    Accelerometer = 0x41,
    Gyroscope = 0x47,
    Light = 0x4C,
    Magnetometer = 0x4D
}