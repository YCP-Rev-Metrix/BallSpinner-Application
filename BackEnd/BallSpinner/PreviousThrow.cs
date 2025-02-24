using System.Collections;
using CsvHelper;
using CsvHelper.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Net.NetworkInformation;
using Common.POCOs;
using System.Data.Common;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;
/// <summary>
/// Interface defining the structure for simulation components in the backend.
/// </summary>
public class PreviousThrow : IBallSpinner
{
    ///<inheritdoc/>
    public string MAC { get; set; } = "Simulation";

    ///<inheritdoc/>
    public DataParser DataParser { get; private set; }

    ///<inheritdoc/>
    public string Name { get; set; } = "Previous Throw";

    ///<inheritdoc/>
    //public event Action? SendErrorToApp;

    ///<inheritdoc/>
    //public event Action? SendRejection;

    ///<inheritdoc/>
    public event Action<bool>? OnConnectionChanged;

    ///<inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public event Action<PhysicalAddress> OnSmartDotMACAddressReceived;

    public string SmartDotMAC { get; } = "11:11:11:11:11:11";
    
    public event Action? SendErrorToApp;
    
    public event Action? SendRejection;

    private string _fileDirectory;

    private List<SampleData?> ShotData = null!;

    // Store enumerator globally so that progress can be tracked if user selects stop
    private IEnumerator<SampleData> enumerator;
    
    // Denotes whether the current previous shot has finished going through all samples
    private bool _isCompleted;

    private bool _running;

    /// <summary>
    /// Constructor for PreviousThrow class. fileDirectory should exist before creating an instance of PreviousThrow
    /// </summary>
    public PreviousThrow(string filePath)
    {
        InitializeConnection();
        _fileDirectory = filePath;
        Name = "Shot: " + Path.GetFileNameWithoutExtension(filePath);
        DataParser = new DataParser();
        ShotData = new List<SampleData>();
        _isCompleted = false;
        GetFileData();
    }

    /// <summary>
    /// 
    /// Disposes of resources used by the simulation component.
    /// </summary>
    public void Dispose()
    {

    }

    ///<inheritdoc/>
    public void InitializeConnection()
    {
        OnConnectionChanged?.Invoke(true);
    }

    public void GetFileData()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord  = false,
        };
        
        using (var reader = new StreamReader(_fileDirectory))
        using (var csv = new CsvReader(reader, config))
        {
            while (csv.Read())
            {
                var record = csv.GetRecord<SampleData>();
                ShotData.Add(record);
            }
        }
        // sort the data by time (ascending)
        ShotData.Sort((x, y) => Nullable.Compare(x.Logtime, y.Logtime));
        // Initialize enumerator. Move to the first element.
        enumerator = ShotData.GetEnumerator();
        enumerator.MoveNext();
    }

    ///<inheritdoc/>
    public bool IsConnected() => true;

    ///<inheritdoc/>
    public void Start()
    {
        // if the user is attempting to start the current shot after already finishing it, replay the shot from
        // the beginning 
        if (_isCompleted || _running)
        {
            return;
            /* - This is all for the future when the reset feature works with BallSpinnerView
            enumerator.Reset();
            enumerator.MoveNext();
            ResetEvent.invoke - when reset works with BallSpinnerViewModel, reset the view so the replay can start
            from beginning
            */
        }
        SampleData? current;
        _running = true;

        double lastTime = enumerator.Current.Logtime!.Value;
        var thread = new Thread(() =>
        {
            while (_running & !_isCompleted)
            {
                // Get current element from enumerator
                current = enumerator.Current;

                Thread.Sleep((int)Math.Abs((lastTime - current.Logtime!.Value) * 1000));
                lastTime = current.Logtime.Value;

                // Light sensor
                if (current.Type == "1")
                {
                    DataParser.DataReceived(Metric.Light, (float)current.X, (float)current.Logtime);
                }
                // Gyroscope
                else if (current.Type == "2")
                {
                    DataParser.DataReceived(Metric.RotationX, (float)current.X, (float)current.Logtime);
                    DataParser.DataReceived(Metric.RotationY, (float)current.Y, (float)current.Logtime);
                    DataParser.DataReceived(Metric.RotationZ, (float)current.Z, (float)current.Logtime);
                }
                // Accelerometer
                else if (current.Type == "3")
                {
                    DataParser.DataReceived(Metric.AccelerationX, (float)current.X, (float)current.Logtime);
                    DataParser.DataReceived(Metric.AccelerationY, (float)current.Y, (float)current.Logtime);
                    DataParser.DataReceived(Metric.AccelerationZ, (float)current.Z, (float)current.Logtime);
                }
                // Magnetometer
                else if (current.Type == "4")
                {
                    DataParser.DataReceived(Metric.MagnetometerX, (float)current.X, (float)current.Logtime);
                    DataParser.DataReceived(Metric.MagnetometerY, (float)current.Y, (float)current.Logtime);
                    DataParser.DataReceived(Metric.MagnetometerZ, (float)current.Z, (float)current.Logtime);
                }
                // Move iterator to next element. If at the end of collection, stop timer event
                if (!enumerator.MoveNext())
                {
                    _isCompleted = true; // Set bool to denote the replay has completed
                    Stop(); // We've made it to the end. Stop the replay
                }
            }

        });

        thread.Start();
    }

    public void ConnectSmartDot(PhysicalAddress? address)
    {
        Console.WriteLine("HI");
    }
    
    ///<inheritdoc/>
    public void Stop()
    {
        _running = false;
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
    public void SubmitSmartDotConfig(double[] ODRs, double[] SampleRates)
    {
        if (!IsSmartDotPaired())
            return;
        //tcp function for sending config message

    }
    /// <inheritdoc/>
    public List<List<double>> GetAvailableRanges()
    {
        throw new NotImplementedException();
    }
    public async void ScanForSmartDots()
    {
        throw new NotImplementedException();
    }

    /// <inheritidoc/>
    public List<List<double>> GetAvailableSampleRates()
    {
        throw new NotImplementedException();
    }
}