using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary/>
    public TCP()
    {
        try
        {
            TcpClient tcpClient = new TcpClient();
            string host = "10.127.7.27";
            int port = 8411;
            var reply = new Ping().Send(host);
            tcpClient.Connect(host, port);

            /*byte[] buffer = new byte[1024];

            tcpClient.Client.Send(Encoding.UTF8.GetBytes("Hello from windows!"));

            while (true)
            {
                int size = tcpClient.Client.Receive(buffer);
                var text = Encoding.UTF8.GetString(buffer, 0, size);
                if (size > 0)
                    Debug.WriteLine(text);
            }*/
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
