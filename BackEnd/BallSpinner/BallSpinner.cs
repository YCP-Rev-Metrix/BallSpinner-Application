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
using Microsoft.Maui;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;

/// <summary>
/// The real, physical ball spinner device
/// </summary>
public class BallSpinnerClass : IBallSpinner 
//Name was changed from BallSpinner to BallSpinnerClass to workaround the namespace having
//the identical name. This allows us to cast to BallSpinner in other files.
{
    /// <inheritdoc/>
    public string Name { get; set; } = "Real Device";

    /// <inheritdoc/>
    public string MAC { get; set; } = "TBD";

    /// <inheritdoc/>
    public DataParser DataParser { get; private set; } = new DataParser();

    public List<double> RPMList { get; set; } = null;

    public int RPMCount { get; set; } = 0;

    public int currentRPMInd { get; set; } = 0;

    ///<inheritdoc/>
    public bool InitialValuesSet => RPMList != null;
    public string SmartDotMAC { get; }

    SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    /// <inheritdoc/>
    public event Action? SendErrorToApp;

    /// <inheritdoc/>
    public event Action? SendRejection;

    // Hardcoded ball FOR NOW
    public Ball ball { get; set; }

    /// <inheritdoc/>
    public event Action<bool>? OnConnectionChanged;
    /// <summary>
    /// Gives an index to the current IBallSpinner implementation. This allows for multiple simulations to be run at the same time
    /// with a separate file name, so that an exception can be avoided.
    /// </summary>
    private int _FileIndex;

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

    public static readonly double[][] RANGE_OPTIONS = {
        [2, 4, 8, 16,-1, -1, -1, -1],
        [125,250,500,1000,2000, -1, -1 ,-1],
        [2500, 4, 8, 16, 8, 16, 32, 64],
        [600, 1300, 8000, 16000, 32000, 64000, -1, -1],
    };

    /// <summary>
    /// The different config settings for sample rates. 
    ///Each index corresponds to an axis measurements
    /// 0 - Accelerometer
    /// 1 - Gyro
    /// 2 - Mag
    /// 3 - Light
    /// </summary>
    public static readonly double[][] SAMPLE_RATES = 
    { 
        //12 should be 12.5 
        [12.5, 25, 50, 100, 200, 400, 800, 1600],
        [25, 50, 100, 200, 400, 800, 1600, 3200, 6400],
        [2, 6, 8, 10, 15, 20, 25, 30, -1], 
        [0.5, 1, 2, 5, 10, 20, -1, -1, -1]
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
    public List<List<double>> AvailableRanges = new List<List<double>>();
    /// <summary>
    /// List of available Sample Rates (Frequency), index 0 - XL, index 1 - GY, index 2 - MG, index 3 - LT
    /// </summary>
    public List<List<double>> AvailableSampleRates = new List<List<double>>();

    /// <summary>
    /// The SmartDot's currently selected Range value for each index.
    /// Each index corresponds to an axis measurements
    /// 0 - Accelerometer
    /// 1 - Gyro
    /// 2 - Mag
    /// 3 - Light
    /// </summary>
    private double[] CurrentRanges = new double[4];

    /// <summary>
    /// The SmartDot's currently selected sample rate value for each index. 
    /// Each index corresponds to an axis measurements
    /// 0 - Accelerometer
    /// 1 - Gyro
    /// 2 - Mag
    /// 3 - Light 
    /// </summary>
    private double[] CurrentSampleRates = new double[4];

    /// <summary>
    /// Integer that is used by multithreaded timer process within Interlocked operations. 
    /// </summary>
    private static int _eventRunning = 0; // Control variable: 0 (idle), 1 (event running), -1 (stopping)
    /// <summary>
    /// Represents the number of discarded events used by the timer process. That is, how many threads attempt to send motor instructions while another thread is currently sending
    /// </summary>
    private static int _discardedEvents = 0;

    /// <summary />
    public BallSpinnerClass(IPAddress address, int FileIndex)
    {
        _address = address;
        _FileIndex = FileIndex;
        InitializeConnection();

        //Me testing - brandon
        //third and 4th index are for testing special gyro output. Should get 3200 and 6400 for gyro
        //gyro should be rate == 25, 6400
        //range should be 1
        byte[] data = { 3, 1, 1, 0b10_00_00_01, 2, 2, 4, 4};
        // Debug.WriteLine($"Range byte: 0b{Convert.ToString(data[0], 2).PadLeft(8, '0')}");

        // SmartDotConfigReceivedEvent(data);

        ball = new Ball("Test", 8.0, 11, "Pancake");

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
    /// <summary>
    /// Get a reference to the TCP connection
    /// </summary>
    /// <returns></returns>
    public TCP GetConnection()
    {
        if(IsConnected())
            return _connection;
        return null;
    }
    private async void OnConnected()
    {
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Connected"));

        Name = await _connection!.GetDeviceInfo();
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Name)));

        _connection!.ScanForSmartDots();
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

    /// <inheritdoc/>
    public async void ScanForSmartDots()
    {
        await _connection!.ScanForSmartDots();
    }

    private void SmartDotConfigReceivedEvent(byte[] data)
    {
        Debug.WriteLine("Triggered the Smart Dot COnfig Received Event!");
        //Convert the data from the bytes and set the AvailableRange and Rate arrays
        
        List<double> XL_rates = new List<double>();
        List<double> GY_rates = new List<double>();
        List<double> MG_rates = new List<double>();
        List<double> LT_rates = new List<double>();

        List<double> XL_ranges = new List<double>();
        List<double> GY_ranges = new List<double>();
        List<double> MG_ranges = new List<double>();
        List<double> LT_ranges = new List<double>();

        int bitLength = 8 - 1; //8 Bits - 1 to offset for array

        // Set XL_rates and XL_ranges
        for (int i = 0; i < 8; i++)
        {
            if ((data[0] & (128 >> i)) != 0) XL_rates.Add(SAMPLE_RATES[0][bitLength - i]);
            if ((data[1] & (128 >> i)) != 0) XL_ranges.Add(RANGE_OPTIONS[0][bitLength - i]);
        }

        // Set GY_rates and GY_ranges
        for (int i = 0; i < 8; i++)
        {
            //GY uses 9 bits for the rates and 7 for the range.
            if ((data[2] & (128 >> i)) != 0) GY_rates.Add(SAMPLE_RATES[1][bitLength - i]);

            if (i != 0)  //only use the 7 bits account for skipping i = 0
                if ((data[3] & (128 >> i)) != 0) GY_ranges.Add(RANGE_OPTIONS[1][(bitLength - i)]);
        }
        //Grab the first bit out of the range byte and use it as the 9th bit for GY_Rates
        if ((data[3] & 128) == 128) GY_rates.Add(SAMPLE_RATES[1][8]);

        // Set MG_rates and MG_ranges
        for (int i = 0; i < 8; i++)
        {
            if ((data[4] & (128 >> i)) != 0) MG_rates.Add(SAMPLE_RATES[2][bitLength - i]);
            if ((data[5] & (128 >> i)) != 0) MG_ranges.Add(RANGE_OPTIONS[2][bitLength - i]);
        }

        // Set LT_rates and LT_ranges
        for (int i = 0; i < 8; i++)
        {
            if ((data[6] & (128 >> i)) != 0) LT_rates.Add(SAMPLE_RATES[3][bitLength - i]);
            if ((data[7] & (128 >> i)) != 0) LT_ranges.Add(RANGE_OPTIONS[3][bitLength - i]);
        }

        //Add the avaialable rates in the correct order to keep the indexing consistent
        AvailableSampleRates.Add(XL_rates);
        AvailableSampleRates.Add(GY_rates);
        AvailableSampleRates.Add(MG_rates);
        AvailableSampleRates.Add(LT_rates);

        //Add the available ranges in the correct order to keep the indexing consistent
        AvailableRanges.Add(XL_ranges);
        AvailableRanges.Add(GY_ranges);
        AvailableRanges.Add(MG_ranges);
        AvailableRanges.Add(LT_ranges);

        foreach (List<double> l in AvailableRanges)
            l.Sort();
        foreach (List<double> l in AvailableSampleRates)
            l.Sort();

        Debug.WriteLine("Available Sample Rates:");
        for (int i = 0; i < AvailableSampleRates.Count; i++)
        {
            Debug.WriteLine($"Index {i}: " + string.Join(", ", AvailableSampleRates[i]));
        }

        Debug.WriteLine("\nAvailable Ranges:");
        for (int i = 0; i < AvailableRanges.Count; i++)
        {
            Debug.WriteLine($"Index {i}: " + string.Join(", ", AvailableRanges[i]));
        }
       
        //Debug.WriteLine("Beginning to send back an artificial message for config");

        //Testing
        //double[] r = new double[4];
        //double[] sr = new double[4];
        //r = [4, 250, 8, 3]; //chosen range values
        //sr = [1600, 6400, 10, 100]; //chosen sample rates
        //SubmitSmartDotConfig(r, sr, true, false, false, true);
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

        DataParser.Start(Name + _FileIndex.ToString());

        currentRPMInd = 0;

        _currentVoltage = 1;
        _motorTimer = new Timer(TimeSpan.FromSeconds(0.010));
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

    /// <summary>
    /// Called from the ballspinner object to disconnect from the BSC
    /// </summary>
    public void DisconnectFromBSC()
    {
        _connection.DisconnectFromBSC();
    }

    /// <summary>
    /// Called from the ballspinner object to toggle the SD taking data
    /// </summary>
    /// <param name="shouldTakeData"></param>
    public void ToggleSDTakeData(bool shouldTakeData)
    {
        _connection.ToggleSDTakeData(shouldTakeData);
    }
    /// <inheritdoc/>
    public void SubmitSmartDotConfig(double[] Ranges, double[] SampleRates, bool XL_OFF, bool GY_OFF, bool MAG_OFF, bool LT_OFF)
    {
        if (!IsSmartDotPaired())
            throw new Exception("Smart Dot must be paired in order to send config settings");

        //Set our current config arrays to the new values.
        CurrentRanges = Ranges;
        CurrentSampleRates = SampleRates;

        //Convert the sample rate into a 4 bit index and the range value into a 4 bit index 
        //Pack these byte index pairs into byte array then message and send it to the pi.
        byte[] bytes = new byte[4];
        for (int i = 0; i < Ranges.Length; i++)
        {
            Debug.WriteLine($"Value of SampleRates[i] == {SampleRates[i]}");
            int sampleRateIndex = GetIndexOfValue(i, SampleRates[i], true);
            Debug.WriteLine($"Value of Ranges[i] == {Ranges[i]}");
            int rangeIndex = GetIndexOfValue(i, Ranges[i], false);

            Debug.WriteLine($"Axis {i}: SampleRateIndex={sampleRateIndex}, RangeIndex={rangeIndex}");

            bytes[i] = Two4BitIntToByte(sampleRateIndex, rangeIndex);
        }

        if (XL_OFF) bytes[0] = 255;
        if (GY_OFF) bytes[1] = 255; 
        if (MAG_OFF) bytes[2] = 255;
        if (LT_OFF) bytes[3] = 255;
        for (int i = 0; i < bytes.Length; i++)
        {
            Debug.WriteLine($"BYTE: [{i}] == {bytes[i]}");

        }
        //TCP send message
        _connection.SendConfigData(bytes);
    }

    ///<summary>
    ///Takes in the chosen values from the UI for the config
    ///index: 0 - XL, 1 - GY, 2 - MG, 3 - LT
    ///</summary>
    private int GetIndexOfValue(int index, double value, bool isSampleRate)
    {
        double[] lookupArray = isSampleRate ? SAMPLE_RATES[index] : RANGE_OPTIONS[index];

        for (int i = 0; i < lookupArray.Length; i++)
        {
            if (lookupArray[i] == value)
                return i; // Return the index when a match is found
        }

        Debug.WriteLine($"Value {value} not found in {(isSampleRate ? "SAMPLE_RATES" : "RANGE_OPTIONS")}[{index}]");
        return -1; // Return -1 if value is not found
    }
    private byte Two4BitIntToByte(int index1, int index2)
    {
        if (index1 < 0 || index1 > 15 || index2 < 0 || index2 > 15)
            throw new ArgumentOutOfRangeException("Both numbers must be between 0 and 15.");

        byte result = (byte)((index1 << 4) | index2);


        //Print as binary
        Debug.WriteLine($" {result} Range byte: 0b{Convert.ToString(result, 2).PadLeft(8, '0')}");
        return result;
    }
    /// <inheritdoc/>
    public List<List<double>> GetAvailableRanges()
    {
        return AvailableRanges;
    }

    /// <inheritidoc/>
    public List<List<double>> GetAvailableSampleRates()
    {
        return AvailableSampleRates;
    }
    /// <inheritidoc/>
    public void SetInitialValues(List<double> RPMs)
    {
        RPMList = RPMs;
        RPMCount = RPMs.Count;
        PropertyChanged.Invoke(null, new PropertyChangedEventArgs("InitialValuesSet"));
    }
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        try
        {
            // Prevent reentrancy: Try to set _eventRunning from 0 to 1 atomically
            if (Interlocked.CompareExchange(ref _eventRunning, 1, 0) != 0)
            {
                Interlocked.Increment(ref _discardedEvents); // Event was skipped due to reentrancy
                Debug.WriteLine("Number of discarded events: " + _discardedEvents);
                return;
            }

            if (currentRPMInd >= RPMCount)
            {
                return; // Don't stop the timer yet—ensure we reset _eventRunning first
            }

            Debug.WriteLine("Current index: " + currentRPMInd);
            byte[] RPMVal = BitConverter.GetBytes((float)RPMList[currentRPMInd]);
            currentRPMInd += 1;
            _connection!.SetMotorRPMs(RPMVal);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            Interlocked.Exchange(ref _eventRunning, 0);
            Interlocked.Exchange(ref _discardedEvents, 0);

            // Ensure the timer is stopped only after _eventRunning is reset
            if (currentRPMInd >= RPMCount)
            {
                _motorTimer.Stop();
            }
        }
    }

}
