using Common.POCOs;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
using CsvHelper.Configuration.Attributes;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;

/// <summary>
/// The real, physical ball spinner device
/// </summary>
public class BallSpinner : IBallSpinner
{
    /// <inheritdoc/>
    public string Name { get; set; } = "Real Device";

    /// <inheritdoc/>
    public string MAC { get; set; } = "TBD";

    /// <inheritdoc/>
    public DataParser DataParser { get; private set; } = new DataParser();

    public string SmartDotMAC { get; }

    /// <inheritdoc/>
    public event Action? SendErrorToApp;

    /// <inheritdoc/>
    public event Action? SendRejection;

    /// <inheritdoc/>
    public event Action<bool>? OnConnectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event Action<PhysicalAddress> OnSmartDotMACAddressReceived;

    private TCP? _connection;
    private IPAddress _address;

    private byte _currentVoltage = 0;
    private Timer? _motorTimer;

    /// <summary>
    /// The different config settings for the Range (+/-)
    /// </summary>
   
    public static readonly int[] RANGE_OPTIONS = [1, 2, 4, 6, 8, 16, 32, 64];

    /// <summary>
    /// The different config settings for sample rates. 
    ///Each index corresponds to an axis measurements
    /// 0 - Accelerometer
    /// 1 - Gyro
    /// 2 - Mag
    /// 3 - Light
    /// </summary>
    public static readonly int[][] SAMPLE_RATES = 
    { 
        [25, 50, 100, 200, 400, 800, 1600, 3200, 6400],
        [25, 50, 100, 200, 400, 800, 1600, 3200, 6400],
        [5, 10, 15, 20, 25, -1, -1, -1, -1], 
        [25, 50, 100, 200, 400, 800, 1600, 3200, 6400]
    };

    // Each index corresponds to an axis measurements
    // 0 - Accelerometer
    // 1 - Gyro
    // 2 - Mag
    // 3 - Light
    // This applies to all of the indicies for the four arrays below

    /// <summary>
    /// List of available Ranges, index 0 - XL, index 1 - GY, index 2 - MG, index 3 - LT
    /// </summary>
    public List<List<int>> AvailableRanges = new List<List<int>>();
    /// <summary>
    /// List of available Sample Rates (Frequency), index 0 - XL, index 1 - GY, index 2 - MG, index 3 - LT
    /// </summary>
    public List<List<int>> AvailableSampleRates = new List<List<int>>();

    /// <summary>
    /// The SmartDot's currently selected Range value for each index.
    /// Each index corresponds to an axis measurements
    /// 0 - Accelerometer
    /// 1 - Gyro
    /// 2 - Mag
    /// 3 - Light
    /// </summary>
    private int[] CurrentRanges = new int[4];

    /// <summary>
    /// The SmartDot's currently selected sample rate value for each index. 
    /// Each index corresponds to an axis measurements
    /// 0 - Accelerometer
    /// 1 - Gyro
    /// 2 - Mag
    /// 3 - Light 
    /// </summary>
    private int[] CurrentSampleRates = new int[4];

    /// <summary />
    public BallSpinner(IPAddress address)
    {
        _address = address;
        InitializeConnection();

        //Me testing - brandon
        //third and 4th index are for testing special gyro output. Should get 3200 and 6400 for gyro
        //gyro should be rate == 25, 6400
        //range should be 1
        byte[] data = { 3, 1, 1, 0b10_00_00_01, 2, 2, 4, 4};
       // Debug.WriteLine($"Range byte: 0b{Convert.ToString(data[0], 2).PadLeft(8, '0')}");

        SmartDotConfigReceivedEvent(data);

    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;

        _motorTimer?.Dispose();
        _motorTimer = null;
    }

    /// <inheritdoc/>
    public async void InitializeConnection()
    {
        if (IsConnected())
            return;

        _connection?.Dispose();
        _connection = new TCP(_address);
        await _connection.Connect();
        
        if (_connection != null && _connection.Connected)
            OnConnected();
    }

    private async void OnConnected()
    {
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Connected"));

        Name = await _connection!.GetDeviceInfo();
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Name)));

        _connection!.ConnectSmartDot(null);
        _connection.SmartDotAddressReceivedEvent += SmartDotAddressReceivedEvent;

        // Subscribe to smartDotRecieved event. Will trigger when a smartdot packet is received
        _connection.SmartDotReceivedEvent += SmartDotRecievedEvent;

        //subscribe to SmartDotConfigReceivedEvent. Will trigger when config packet is received
        _connection.SmartDotConfigReceivedEvent += SmartDotConfigReceivedEvent;

        OnConnectionChanged?.Invoke(true);


    }

    /// <inheritdoc/>
    public async void ConnectSmartDot(PhysicalAddress? address)
    {
        await _connection!.ConnectSmartDot(address);
    }

    private void SmartDotConfigReceivedEvent(byte[] data)
    {
        Debug.WriteLine("Triggered the Smart Dot COnfig Received Event!");
        //Convert the data from the bytes and set the AvailableRange and Rate arrays

        //Get the bits from our byte array
        BitArray bitArray = new BitArray(data);

        for (int i = 0; i < bitArray.Length; i += 16)
        {
            //If i == 16, then we are on the GY. It has a special case, where we need first 9 bits for Rate and last 7 for Range
            int firstEnd = (i == 16) ? 9 : 8;

            //Grab the first 8 bits from the byte, 
            //Iterate through available Rates and add to that index to list if its 1 add it to available list
            List<int> rates = new List<int>();
            for (int j = 0; j < firstEnd; j++)
            {
                int index = i + j;
                if (bitArray[i + j])
                {
                    rates.Add(SAMPLE_RATES[i / 16][j]);
                   // Debug.WriteLine($"bit i = {i} i/16 = {i / 16} j = {j} , array result {SAMPLE_RATES[i / 16][j]}, ");

                }
            }
            //Add the list to the list of lists
            AvailableSampleRates.Add(rates);

            //Do the same for the Range
            List<int> range = new List<int>();
            for (int j = firstEnd; j < 16; j++)
            {
                if (bitArray[i + j])
                {
                    range.Add(RANGE_OPTIONS[j - 8]);
                }
            }
            AvailableRanges.Add(range);

        }
        //Debug.WriteLine("Available Sample Rates:");
        //for (int i = 0; i < AvailableSampleRates.Count; i++)
        //{
        //    Debug.WriteLine($"Index {i}: " + string.Join(", ", AvailableSampleRates[i]));
        //}

        //Debug.WriteLine("\nAvailable Ranges:");
        //for (int i = 0; i < AvailableRanges.Count; i++)
        //{
        //    Debug.WriteLine($"Index {i}: " + string.Join(", ", AvailableRanges[i]));
        //}

    }
    private void SmartDotAddressReceivedEvent(PhysicalAddress address)
    {
        Debug.WriteLine("Device address: " + address.ToString() + " This has extra bytes from the product name");
        OnSmartDotMACAddressReceived?.Invoke(address);
    }

    private void SmartDotRecievedEvent(SensorType sensorType, float timeStamp, int sampleCount, float XData, float YData, float ZData)
    {
        DataParser.SendSmartDotToSubscribers(sensorType, timeStamp, sampleCount, XData, YData, ZData);
    }
    

    /// <inheritdoc/>
    public bool IsConnected()
    {
        return _connection != null && _connection.Connected;
    }

    /// <inheritdoc/>
    public void ResendMessage()
    {
        
    }

    /// <inheritdoc/>
    public List<string> SendBackListOfSmartDots()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetSmartDot()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Start()
    {
        //Stop();

        DataParser.Start(Name);

        _currentVoltage = 1;
        _motorTimer = new Timer(TimeSpan.FromSeconds(0.25));
        _motorTimer.Elapsed += OnTimedEvent;
        _motorTimer.Start();

    }

    /// <inheritdoc/>
    public void Stop()
    {
        _motorTimer?.Dispose();
        _motorTimer = null;

        //_connection?.SetMotorVoltages(0, 0, 0);
        _connection?.StopMotorInstructions();

        DataParser.Stop();
    }

    /// <inheritdoc/>
    public bool IsSmartDotPaired()
    {
        if (!IsConnected())
            return false;

        if (!string.IsNullOrEmpty(SmartDotMAC))
            return false;
        
        return true;
    }

    /// <inheritdoc/>
    public void SubmitSmartDotConfig(int[] Ranges, int[] SampleRates)
    {
        if (!IsSmartDotPaired())
            throw new Exception("Smart Dot must be paired in order to send config settings");
        

        //Set our current config arrays to the new values.
        CurrentRanges = Ranges;
        CurrentSampleRates = SampleRates;

        //Convert the range value into a byte we can send to the pi
        byte[] bytes = new byte[4];
        for (int i = 0; i < Ranges.Length; i++)
            bytes[i] = Two4BitIntToByte(Ranges[i], SampleRates[i]);

        _connection.SendConfigDataAndStartTakeData(bytes);
    }

    private byte Two4BitIntToByte(int index1, int index2)
    {
        if (index1 < 0 || index1 > 15 || index2 < 0 || index2 > 15)
            throw new ArgumentOutOfRangeException("Both numbers must be between 0 and 15.");

        byte result = (byte)((index1 << 4) | index2);


        //Print as binary
        Debug.WriteLine($"Range byte: 0b{Convert.ToString(result, 2).PadLeft(8, '0')}");
        return result;
    }
    /// <inheritdoc/>
    public int[] GetAvailableRanges()
    {
        throw new NotImplementedException();
    }

    /// <inheritidoc/>
    public int[] GetAvailableSampleRates()
    {
        throw new NotImplementedException();
    }
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        if (!IsConnected())
            return;

        _currentVoltage++;

        if (_currentVoltage >= 30) //Primary motor supports up to 30, secondary motors only 12
            _currentVoltage = 0;

        byte x, y, z;

        if (_currentVoltage >= 0 && _currentVoltage <= 10)
            y = 8;
        else
            y = 0;

        if (_currentVoltage >= 10 && _currentVoltage <= 20)
            z = 8;
        else
            z = 0;

        if (_currentVoltage >= 15 && _currentVoltage <= 30)
            x = _currentVoltage;
        else
            x = 0;

        //x = 30;

        _connection!.SetMotorVoltages(x, y, z);
    }
}
