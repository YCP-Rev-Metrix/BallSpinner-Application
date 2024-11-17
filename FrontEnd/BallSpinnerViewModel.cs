using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    private IBallSpinner _ballSpinner;

    public IDataViewModel LeftView { get; }

    public IDataViewModel TopMiddleView { get; }

    public IDataViewModel BottomMiddleView { get; }

    public IDataViewModel TopRightView { get; }

    public IDataViewModel BottomRightView { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainPage MainPage { get; }

    public BallSpinnerViewModel(MainPage mainPage, IBallSpinner ballspinner)
    {
        MainPage = mainPage;
        _ballSpinner = ballspinner;

        LeftView = new BallViewModel(_ballSpinner);
        TopMiddleView = new GraphViewModel(_ballSpinner, "Acceleration", Metric.AccelerationX | Metric.AccelerationY | Metric.AccelerationZ);
        BottomMiddleView = new GraphViewModel(_ballSpinner, "Rotation", Metric.RotationX | Metric.RotationY | Metric.RotationZ);
        TopRightView = new GraphViewModel(_ballSpinner, "Magnetometer", Metric.MagnetometerX | Metric.MagnetometerY | Metric.MagnetometerZ);
        BottomRightView = new GraphViewModel(_ballSpinner, "Light", Metric.Light);

        _ballSpinner.PropertyChanged += _ballSpinner_PropertyChanged;
        _ballSpinner.OnConnectionChanged += _ballSpinner_OnConnectionChanged;

        NotConnectedFadeVisible = !_ballSpinner.IsConnected();
    }

    private void _ballSpinner_OnConnectionChanged(bool connected)
    {
        NotConnectedFadeVisible = !connected;
    }

    private void _ballSpinner_PropertyChanged(object? sender, PropertyChangedEventArgs e)
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
        _ballSpinner.PropertyChanged -= _ballSpinner_PropertyChanged;
        _ballSpinner.OnConnectionChanged -= _ballSpinner_OnConnectionChanged;

        _ballSpinner.Dispose();
    }
}
