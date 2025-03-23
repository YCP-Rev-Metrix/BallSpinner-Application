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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    /// <summary>
    /// Invoked when config options are received from SD. Sends byte array to BallSpinner for interpretation
    /// </summary>
    public event Action<byte[]> SmartDotConfigReceivedEvent;

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
        _send[0] = (byte)MessageType.A_B_START_SCAN_FOR_SD;
        await _client.Client.SendAsync(new ArraySegment<byte>(_send, 0, 1));
    }

    /// <summary>
    /// Choose a smart dot module to connect to.
    /// </summary>
    public async Task ConnectSmartDot(PhysicalAddress? address)
    {
        _send[0] = (byte)MessageType.A_B_CHOSEN_SD;
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

                        case MessageType.A_B_START_SCAN_FOR_SD:
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
                        case MessageType.B_A_RECEIVE_CONFIG_INFO:
                            var bytes = new ArraySegment<byte>(packetFixed, 3, messageSize).ToArray();
                            Debug.WriteLine($"Config Message received, size = {messageSize}");
                            //Call event on the Ball Spinner
                            Console.WriteLine("Hex Output: " + BitConverter.ToString(bytes));
                            SmartDotConfigReceivedEvent?.Invoke(bytes);

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

        byte type = (byte)MessageType.A_B_MOTOR_INSTRUCTIONS;
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
    /// Sends RPMS to the motors
    /// The Rpm parameter is expected to be a 32 bit float value conerted to a byte array
    /// </summary>
    public async void SetMotorRPMs(byte[] Rpm)
    {
        if (!_client.Connected)
            throw new Exception("Can't send instructions without being connected");

        try
        {
            byte type = (byte)MessageType.A_B_MOTOR_INSTRUCTIONS;
            byte[] instructions = new byte[]
            {
           // (byte)(inx & 0xFF), // Lower byte of inx
           //(byte)((inx >> 8) & 0xFF), // Second byte of inx
           //(byte)((inx >> 16) & 0xFF), // Third byte of inx
           //(byte)((inx >> 24) & 0xFF), // Fourth byte of inx - Debug stuff

            type, //type

            0x00,
            0x03, // size

            Rpm[0], // Set driver motor RPM value
            Rpm[1],
            Rpm[2],
            Rpm[3],

            0x00, //Motor 2 values
            0x00,
            0x00,
            0x00,

            0x00, // Motor 3 values
            0x00,
            0x00,
            0x00,
            };
            // Send the motor instruction to the PI
            await _client.Client.SendAsync(instructions);
            Debug.WriteLine(BitConverter.ToString(instructions));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
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
    /// <summary>
    /// Send a message to disconnect the BSC from the App.
    /// </summary>
    public async void DisconnectFromBSC()
    {
        if (!_client.Connected)
            throw new Exception("Can't send disconnect without being connected");

        byte type = (byte)MessageType.A_B_DISCONNECT_FROM_BSC;
        //Sends a STOP_MOTOR_INSTUCTIONS per Roberts Protocol sheet
        byte[] instructions = new byte[]
        {
            type,
        };

        // Send the disconnect message to the PI
        await _client.Client.SendAsync(instructions);
    }

    /// <summary>
    /// Send indices of chosen data config of the SD to the BSC
    /// </summary>
    public async void SendConfigData(byte[] bytes)
    {
        if (!_client.Connected)
            throw new Exception("Can't send config without being connected");

        byte type = (byte)MessageType.A_B_RECEIVE_CONFIG_INFO;
        byte[] message = new byte[]
        {
            type,

            0x00,
            0x08,//size

            //XL - Frequency(byte), Range(byte)
            bytes[0],
            //GY - Frequency(byte), Range(byte)
            bytes[1],
            //MG - Frequency(byte), Range(byte)
            bytes[2],
            //LT - Frequency(byte), Range(byte)
            bytes[3],
        };
        await _client.Client.SendAsync(message);
    }

    ///<summary>
    /// Set the SD to take/send data. Pass in true to take data and false to stop.
    /// </summary>
    public async void ToggleSDTakeData(bool shouldTakeData)
    {
        if (!_client.Connected)
            throw new Exception("Can't send instructions without being connected");

        byte type = (byte)MessageType.A_B_SD_TOGGLE_TAKE_DATA;
        //Sends a STOP_MOTOR_INSTUCTIONS per Roberts Protocol sheet

        //Set our data byte to be all 1s or 0s
        byte data = shouldTakeData ? (byte)255 : (byte)0;

        byte[] instructions = new byte[]
        {
            type,
            data,
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
    A_B_START_SCAN_FOR_SD = 0x05,

    /// <summary>
    /// Response from server containing smart dot connection info, sent for each SmartDot module found by A_B_SCAN_FOR_SD
    /// </summary>
    B_A_SCANNED_SD = 0x06,

    /// <summary>
    /// Send the chosen smart dot MAC address to the BSC to confirm the choice.
    /// </summary>
    A_B_CHOSEN_SD = 0x07,

    /// <summary>
    /// Receives 2 byte pairs that are used to say which values from the SmartDot Config bitmaps are valid options
    /// </summary>
    B_A_RECEIVE_CONFIG_INFO = 0x08,

    /// <summary>
    /// Sends 2 byte pairs that with each byte only having one 1. 
    /// This message selects the desired config options and sends them to the Ball Spinner Controller
    /// </summary>
    A_B_RECEIVE_CONFIG_INFO = 0x09,

    /// <summary>
    /// Indicates a SmartDot data packet
    /// </summary>
    B_A_SD_SENSOR_DATA = 0x0B,

    /// <summary>
    /// Set Motor Voltages packet type
    /// </summary>
    A_B_MOTOR_INSTRUCTIONS = 0x0C,

    /// <summary>
    /// Stop the motors on the BSC
    /// </summary>
    A_B_STOP_MOTOR = 0x0D,

    ///<summary>
    /// Disconnect from the BSC
    /// </summary>
    A_B_DISCONNECT_FROM_BSC = 0x0E,

    ///<summary>
    /// Enable or disable SD taking/sending data.
    /// </summary>
    A_B_SD_TOGGLE_TAKE_DATA = 0x0F,

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