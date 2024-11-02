using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;
/// <summary>
/// Interface defining the structure for simulation components in the backend.
/// </summary>
public class Simulation : IBallSpinner
{
    ///<inheritdoc/>
    public string MAC { get; set; } = "Simulation";

    ///<inheritdoc/>
    public DataParser DataParser { get; } = new DataParser();

    ///<inheritdoc/>
    public string Name { get; set; } = "Simulation";

    ///<inheritdoc/>
    public event Action? SendErrorToApp;

    ///<inheritdoc/>
    public event Action? SendRejection;

    private Timer _timer = null!;

    /// <summary>
    /// 
    /// Disposes of resources used by the simulation component.
    /// </summary>
    public void Dispose() { }

    ///<inheritdoc/>
    public void InitializeConnection()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public bool IsConnected()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public void ResendMessage()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public List<string> SendBackListOfSmartDots()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public void SetSmartDot()
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public void Start()
    {
        TimeSpan frequency = TimeSpan.FromSeconds(1 / 60f);
        _timer = new Timer((o) =>
        {
            float time = (float)DateTime.UtcNow.TimeOfDay.TotalSeconds;
            DataParser.DataReceived(Metric.RotationX, MathF.Sin(time), time);
            DataParser.DataReceived(Metric.RotationY, MathF.Cos(time), time);
            DataParser.DataReceived(Metric.RotationZ, -MathF.Sin(time), time);
        }, null, frequency, frequency);
    }

    ///<inheritdoc/>
    public void Stop()
    {
        _timer?.Dispose();
    }
}
