using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.FrontEnd.Pages;
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
    private Window? _helpWindow;
    private Window? _newBallSpinnerWindow;
    private Window? _newLoginWindow;

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
        var window = new Window(new AlertPage(message))
        {
            Title = "Alert"
        };

        Application.Current!.OpenWindow(window);
    }

    /// <summary>
    /// Opens the help window
    /// </summary>
    public void Help()
    {
        if (_helpWindow != null)
            return;

        _helpWindow = new Window(new HelpView())
        {
            Title = "Help"
        };
        Application.Current!.OpenWindow(_helpWindow);
        _helpWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _helpWindow = null;
        };
    }

    public async Task<IBallSpinner?> AddBallSpinner()
    {
        if (_newBallSpinnerWindow != null)
            return null;

        TaskCompletionSource<IBallSpinner?> task = new TaskCompletionSource<IBallSpinner?>();
        var newBallSpinnerView = new NewBallSpinnerView(task);
        _newBallSpinnerWindow = new Window(newBallSpinnerView)
            {
                Title = "Add Ball Spinner"
            };
        Application.Current!.OpenWindow(_newBallSpinnerWindow);
        _newBallSpinnerWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _newBallSpinnerWindow = null;

            if(!task.Task.IsCompleted)
                task.SetResult(null);
        };

        var result = await task.Task;
        if(_newBallSpinnerWindow != null)
            Application.Current.CloseWindow(_newBallSpinnerWindow);

        return result;
    }

    public void Login()
    {
        if (_newLoginWindow != null)
            return;

        _newLoginWindow = new Window(new LoginPage(this, Backend.Database))
        {
            Title = "LoginPage",
            Width = 300,
            Height = 400,
            X = 100,
            Y = 100
    };
        Application.Current!.OpenWindow(_newLoginWindow);
        _newLoginWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _newLoginWindow = null;
        };
    }
}
