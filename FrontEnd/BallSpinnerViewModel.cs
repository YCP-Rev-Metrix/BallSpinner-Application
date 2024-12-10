using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;
public partial class BallSpinnerViewModel : INotifyPropertyChanged, IDisposable
{
    public bool NotConnectedFadeVisible
    {
        get => _connectedFadeVisible;
        set
        {
            _connectedFadeVisible = value;
            OnPropertyChanged(nameof(NotConnectedFadeVisible));
        }
    }
    private bool _connectedFadeVisible = true;

    public string Name 
    {
        get => _ballSpinner.Name;
        set
        {
            _ballSpinner.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public IBallSpinner BallSpinner => _ballSpinner;

    private IBallSpinner _ballSpinner;

    public IDataViewModel LeftView { get; }

    public IDataViewModel TopMiddleView { get; }

    public IDataViewModel BottomMiddleView { get; }

    public IDataViewModel TopRightView { get; }

    public IDataViewModel BottomRightView { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainPage MainPage { get; }

    private FrontEnd _frontEnd;

    public BallSpinnerViewModel(FrontEnd frontend, MainPage mainPage, IBallSpinner ballspinner)
    {
        _frontEnd = frontend;
        MainPage = mainPage;
        _ballSpinner = ballspinner;

        LeftView = new BallViewModel(_ballSpinner);
        TopMiddleView = new GraphViewModel(_ballSpinner, "Acceleration (g)", Metric.AccelerationX | Metric.AccelerationY | Metric.AccelerationZ);
        BottomMiddleView = new GraphViewModel(_ballSpinner, "Rotation (°)", Metric.RotationX | Metric.RotationY | Metric.RotationZ);
        TopRightView = new GraphViewModel(_ballSpinner, "Magnetometer (μT)", Metric.MagnetometerX | Metric.MagnetometerY | Metric.MagnetometerZ);
        BottomRightView = new GraphViewModel(_ballSpinner, "Light (lux)", Metric.Light);

        _ballSpinner.PropertyChanged += BallSpinner_PropertyChanged;
        _ballSpinner.OnConnectionChanged += BallSpinner_OnConnectionChanged;

        BallSpinner_OnConnectionChanged(_ballSpinner.IsConnected());
    }

    private async void BallSpinner_OnConnectionChanged(bool connected)
    {
        NotConnectedFadeVisible = !connected;

        if(connected && string.IsNullOrEmpty(_ballSpinner.SmartDotMAC))
        {
            await _frontEnd.ConnectSmartDot(_ballSpinner);
        }
    }

    private void UpdateConnected(bool connected)
    {
        NotConnectedFadeVisible = !connected | string.IsNullOrEmpty(_ballSpinner.SmartDotMAC);
    }

    private void BallSpinner_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    public void Start()
    {
        _ballSpinner.Start();
    }

    public void Stop()
    {
        _ballSpinner.Stop();
    }

    public void Reconnect()
    {
        _ballSpinner.InitializeConnection();
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _ballSpinner.PropertyChanged -= BallSpinner_PropertyChanged;
        _ballSpinner.OnConnectionChanged -= BallSpinner_OnConnectionChanged;

        _ballSpinner.Dispose();
    }
}
