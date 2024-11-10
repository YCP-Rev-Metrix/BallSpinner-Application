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

namespace RevMetrix.BallSpinner.BackEnd;

/// <summary>
/// TCP client that connects to the ball spinner device
/// </summary>
public class TCP : IDisposable
{
    private TcpClient _client;
    private IPAddress _address;

    private const int BUFFER_SIZE = 1024;
    private const ushort PORT = 8411;

    /// <summary/>
    public TCP(IPAddress address)
    {
        _client = new TcpClient();
        _address = address;
        Task.Run(Connect);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client.Dispose();
    }

    private async Task Connect()
    {
        try
        {
            await _client.ConnectAsync(_address, PORT);
            await _client.Client.SendAsync(Encoding.UTF8.GetBytes("Hello from the application!"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Listen();
    }

    private async void Listen()
    {
        while (_client.Connected)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int size = await _client.Client.ReceiveAsync(buffer);

            if (size <= 0)
                return;

            //Format
            //Byte 0 = Message type
            //Byte 1-2 = Message size (Maximum 65,536 bytes)
            //

            byte messageType = buffer[0];
            int messageSize = buffer[1] + (buffer[2] << sizeof(byte));

            if (size > 0)
                Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, size));
        }
    }
}
