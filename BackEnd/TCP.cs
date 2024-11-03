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
public class TCP
{
    private TcpClient _client;

    /// <summary/>
    public TCP()
    {
        _client = new TcpClient();

        Connect();
        Listen();
    }

    private async void Connect()
    {
        try
        {
            _client = new TcpClient();
            string host = "10.127.7.20";
            int port = 8411;
            var reply = new Ping().Send(host);
            await _client.ConnectAsync(host, port);

            await _client.Client.SendAsync(Encoding.UTF8.GetBytes("Hello from the application!"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async void Listen()
    {
        while (_client.Connected)
        {
            byte[] buffer = new byte[1024];
            int size = await _client.Client.ReceiveAsync(buffer);

            if (size > 0)
                Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, size));
        }
    }
}
