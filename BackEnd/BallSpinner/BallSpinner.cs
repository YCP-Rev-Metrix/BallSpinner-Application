using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public DataParser DataParser { get; } = new DataParser();

    /// <inheritdoc/>
    public event Action? SendErrorToApp;

    /// <inheritdoc/>
    public event Action? SendRejection;

    private TCP? _connection;

    /// <summary />
    public BallSpinner()
    {
        InitializeConnection();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _connection?.Dispose();
    }

    /// <inheritdoc/>
    public void InitializeConnection()
    {
        if (IsConnected())
            return;

        _connection = new TCP();
    }

    /// <inheritdoc/>
    public bool IsConnected()
    {
        return _connection != null;
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
        //message ball spinner
    }

    /// <inheritdoc/>
    public void Stop()
    {
        //message ball spinner
    }
}
