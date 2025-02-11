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
        _pendingMessages.Add((MessageType.B_A_NAME, task));

        _send[0] = (byte)MessageType.A_B_NAME_REQ;
        _send[1] = 0;
        _send[2] = 0;
        await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 3));

        var result = await task.Task;
        return (string)result;
    }
    

    ///<summary>
    /// Tell the Ball Spinner Controller to start scanning available Smart Dot Modules. The Ball Spinner will then send their data back to this App
    /// TO BE IMPLEMENTED
    /// </summary>
    public async Task ScanForSmartDots()
    {
        //NOT YET USED (still using ConnectSmartDot to begin listening to packets
        _send[0] = (byte)MessageType.A_B_SCAN_FOR_SD;
        await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 1));
    }

    /// <summary>
    /// Choose a smart dot module to connect to. Sending null indicates you want to start listening to packets
    /// </summary>
    public async Task ConnectSmartDot(PhysicalAddress? address)
    {
        _send[0] = (byte)MessageType.A_B_SCAN_FOR_SD;
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
            _send[0] = (byte)MessageType.A_B_INIT_HANDSHAKE;
            _send[2] = 1; //No data to send
            _send[3] = token; //Random token, BallSpinner MUST send back the same value

            int sent = await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 4));
            if (sent <= 0)
                throw new Exception("Handshake request could not be sent");

            //Receive ball spinner data, if data is not receieved the connection was rejected
            int receieved = await _client.Client.ReceiveAsync(_receive);
            if (receieved <= 0)
                throw new Exception("Handshake request could not be received");

            if (_receive[0] != (byte)MessageType.B_A_INIT_HANDSHAKE_ACK)
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
                    // Check to see if multiple packets are being process at the same time
                    Debug.WriteLine($"{BitConverter.ToString(packetFixed)}");
                    currentIndex += packetFixed.Length;

                    var (messageType, messageSize) = GetMessageInfo(packetFixed);
                    _receive[currentIndex] = 0;

                    switch (messageType)
                    {
                        case MessageType.A_B_NAME_REQ:
                        case MessageType.B_A_NAME:

                            string text = Encoding.UTF8.GetString(packetFixed, 3, messageSize);
                            Debug.WriteLine(text);

                            var task = TryGetResponse(messageType);
                            task?.SetResult(text);

                            break;

                        case MessageType.A_B_SCAN_FOR_SD:
                        case MessageType.B_A_SCANNED_SD:
                            var physicalAddressBytes = new ArraySegment<byte>(packetFixed, 3, messageSize).ToArray();

                            var physicalAddress = new PhysicalAddress(physicalAddressBytes);
                            SmartDotAddressReceivedEvent?.Invoke(physicalAddress);
                            break;
                        case MessageType.B_A_SD_SENSOR_DATA:
                            SensorType sensorType = (SensorType)Enum.ToObject(typeof(SensorType), packetFixed[3]);
                            int sampleCount = packetFixed[6] | (packetFixed[5] << 8) | (packetFixed[4] << 16); //3 bytes
                            float timeStamp = BitConverter.ToSingle(packetFixed, 7); //BitConverter expects LITTLE ENDIAN
                            float xData = BitConverter.ToSingle(packetFixed, 11);
                            float yData = BitConverter.ToSingle(packetFixed, 15);
                            float zData = BitConverter.ToSingle(packetFixed, 19);
                            // Debug statement to filter out light data (it comes in too slow right now)
                            if (sensorType == SensorType.Light) {
                                Debug.WriteLine($"{sensorType}: {xData} {timeStamp} {sampleCount}");
                            }
                            // Debug statement to print incoming smartdot packet data
                            //Debug.WriteLine($"{sensorType}: {xData} {yData} {zData}");
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

        byte type = (byte)MessageType.A_B_SEND_MOTOR_VOLTAGES;
        byte[] instructions = new byte[]
        {
            type, //Type

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

        byte type = (byte)MessageType.A_B_STOP_MOTOR;
        //Sends a STOP_MOTOR_INSTUCTIONS per Roberts Protocol sheet
        byte[] instructions = new byte[]
        {
            type, 
        };

        // Send the motor instruction to the PI
        await _client.Client.SendAsync(instructions);
    }
}


/// <summary>
/// Types of messages that can be sent to the ball spinner
/// First 2 bits = Version number
/// Last 6 bits = Message Type
/// A_B -> App to Ball Spinner Controller
/// B_A -> Ball Spinner Controller to App
/// SD ->SMARTDOT(s)
/// </summary>
public enum MessageType : byte
{
    /// <summary>
    /// Request connection to the Ball Spinner Controller
    /// </summary>
    A_B_INIT_HANDSHAKE = 0x01,

    /// <summary>
    /// Incoming acknowledgement of the connection to the Ball Spinner Controller
    /// </summary>
    B_A_INIT_HANDSHAKE_ACK = 0x02,

    /// <summary>
    /// Request for device name
    /// </summary>
    A_B_NAME_REQ = 0x03,

    /// <summary>
    /// Response from Ball Spinner Controller containing device name
    /// </summary>
    B_A_NAME = 0x04,


    /// <summary>
    /// Tell the Ball Spinner Controller to scan for available SmartDot modules
    /// </summary>
    A_B_SCAN_FOR_SD = 0x05,

    /// <summary>
    /// Response from server containing smart dot connection info, sent for each SmartDot module found by A_B_SCAN_FOR_SD
    /// </summary>
    B_A_SCANNED_SD = 0x06,
    
    /// <summary>
    /// Indicates a SmartDot data packet
    /// </summary>
    B_A_SD_SENSOR_DATA = 0x0A,

    /// <summary>
    /// Set Motor Voltages packet type
    /// </summary>
    A_B_SEND_MOTOR_VOLTAGES = 0x08,

    /// <summary>
    /// Stop the motors on the BSC
    /// </summary>
    A_B_STOP_MOTOR = 0x08,

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