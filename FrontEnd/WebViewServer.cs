using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// Simple web server for serving pages to local MAUI <see cref="WebView"/>s
/// </summary>
public class WebViewServer : IDisposable
{
    private const string NOT_ALLOWED = "Path is not allowed";
    private const string NOT_EXIST = "Path does not exist";

    private const string URI_HOST = "localhost";
    private const string URI_SCHEME = "http";
    private const int URI_PORT = 4837; //random
    private const int MAX_REQUESTS = 8;

    private HttpListener _listener;
    private Dictionary<string, CachedResult> _fileCache = new ();

    public WebViewServer()
    {
        Console.WriteLine("WebViewServer Init");

        if (!HttpListener.IsSupported)
            throw new NotSupportedException($"{nameof(HttpListener)} is not supported, front end will not function.");
        
        string uri = new UriBuilder(URI_SCHEME, URI_HOST, URI_PORT).ToString();
        _listener = new HttpListener();
        _listener.Prefixes.Add(uri);

        StartListening();
    }

    private async void StartListening()
    {
        _listener.Start();

        var requests = new HashSet<Task>();
        for (int i = 0; i < MAX_REQUESTS; i++)
            requests.Add(_listener.GetContextAsync());

        while (true)
        {
            Task t = await Task.WhenAny(requests);
            requests.Remove(t);

            if (t is Task<HttpListenerContext> request)
            {
                var context = request.Result;
                requests.Add(ProcessRequestAsync(context));
                requests.Add(_listener.GetContextAsync());
            }
        }
    }

    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        string path = context.Request.RawUrl!.Remove(0, 1);
        byte[] response;
        string contentType = string.Empty;
        if(_fileCache.TryGetValue(path, out var cachedResult))
        {
            response = cachedResult.Bytes;
            contentType = cachedResult.ContentType;
        }
        else
        {
            string extension = Path.GetExtension(path);
            
            switch (extension)
            {
                case ".html":
                    contentType = "text/html";
                    break;
                case ".js":
                    contentType = "text/javascript";
                    break;
                case ".mjs":
                    contentType = "text/javascript";
                    break;
            }

            string responseText;
            if (string.IsNullOrEmpty(contentType))
            {
                responseText = NOT_ALLOWED;
            }
            else if (await FileSystem.AppPackageFileExistsAsync(path) == false)
            {
                responseText = NOT_EXIST;
            }
            else
            {
                using Stream file = await FileSystem.OpenAppPackageFileAsync(path);
                using StreamReader reader = new StreamReader(file);
                responseText = await reader.ReadToEndAsync();
            }

            response = Encoding.UTF8.GetBytes(responseText);
            _fileCache[path] = new CachedResult
            {
                Bytes = response,
                ContentType = contentType
            };
        }

        context.Response.ContentType = contentType;
        context.Response.ContentLength64 = response.LongLength;
        await context.Response.OutputStream.WriteAsync(response);

        context.Response.Close();
    }

    public void Dispose()
    {
        _listener.Close();
    }

    internal struct CachedResult 
    {
        /// <summary>
        /// UTF-8 byte representation of string
        /// </summary>
        public byte[] Bytes;

        /// <summary>
        /// Type of the content, such as 'text/html'
        /// </summary>
        public string ContentType;
    }
}
