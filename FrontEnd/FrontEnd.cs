using RevMetrix.BallSpinner.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// Core front end that is used for visualizing a <see cref="Backend"/>
/// </summary>
public class FrontEnd : IFrontEnd
{
    /// <summary>
    /// The core application that the front end will visualize
    /// </summary>
    public Backend Backend { get; private set; } = null!;

    private WebViewServer _webViewServer;

    /// <summary>
    /// Called before <see cref="Backend"/> is initialized
    /// </summary>
    public FrontEnd()
    {
        Console.WriteLine("FrontEnd Pre-Init");

        _webViewServer = new WebViewServer();
    }

    /// <summary>
    /// Called after <see cref="Backend"/> is initialized
    /// </summary>
    public void Init(Backend backend)
    {
        Console.WriteLine("FrontEnd Init");

        Backend = backend;
    }
    
    public void Dispose()
    {
        _webViewServer.Dispose();
    }

    public void Alert(string message)
    {
        var window = new Window(new AlertPage(message));
        window.Title = "Alert";
        
        Application.Current!.OpenWindow(window);
    }
}
