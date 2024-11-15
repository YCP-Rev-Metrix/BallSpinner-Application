﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    private IPAddress _address;
    /// <summary />
    public BallSpinner(IPAddress address)
    {
        _address = address;
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

        _connection = new TCP(_address);
        // Subscribe to OnDataRecievedEvent
        _connection.OnDataRecieved += OnDataRecievedEventHandler;
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

    // Called when TCP class recieves a packet. Send packet to Dataparser parse it. 
    public void OnDataRecievedEventHandler(byte[] packet)
    {
        DataParser.ParsePacket(packet);
    }
}
