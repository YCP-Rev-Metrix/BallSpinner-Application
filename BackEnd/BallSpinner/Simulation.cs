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
    public bool IsConnection()
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
        _timer = new Timer((o) =>
        {
            DataParser.DataReceived(Metric.RotationX, (float)DateTime.UtcNow.TimeOfDay.TotalSeconds, (float)DateTime.UtcNow.TimeOfDay.TotalSeconds);
        }, null, TimeSpan.FromSeconds(0.25f), TimeSpan.FromSeconds(0.25f));
    }

    ///<inheritdoc/>
    public void Stop()
    {
        throw new NotImplementedException();
    }
}
