using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.FrontEnd.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
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
    private Window? _newInitialValuesWindow;
    private Window? _newCloudManagementWindow;
    private Window? _newArsenalWindow;
    private Window? _newSmartDotSettingsWindow;

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

    public async Task<IBallSpinner?> AddBallSpinner(int BallSpinnerCount, int SimulationCount)
    {
        if (_newBallSpinnerWindow != null)
            return null;

        TaskCompletionSource<IBallSpinner?> task = new TaskCompletionSource<IBallSpinner?>();
        var newBallSpinnerView = new NewBallSpinnerView(task, BallSpinnerCount, SimulationCount);
        _newBallSpinnerWindow = new Window(newBallSpinnerView)
        {
            Title = "Add Ball Spinner",
            Width = 600,
            Height = 400,
            X = 100,
            Y = 100
        };
        Application.Current!.OpenWindow(_newBallSpinnerWindow);
        _newBallSpinnerWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _newBallSpinnerWindow = null;

            if (!task.Task.IsCompleted)
                task.SetResult(null);
        };

        var result = await task.Task;
        if (_newBallSpinnerWindow != null)
            Application.Current.CloseWindow(_newBallSpinnerWindow);

        return result;
    }

    public void Login()
    {
        if (_newLoginWindow != null)
            return;

        _newLoginWindow = new Window(new LoginPage(this, Backend.Database))
        {
            Title = "Login",
            Width = 600,
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

    public void CloseLoginWindow()
    {
        if (_newLoginWindow != null)
        {
            Application.Current!.CloseWindow(_newLoginWindow);
            _newLoginWindow = null;
        }
    }

    public async Task<PhysicalAddress?> ConnectSmartDot(IBallSpinner spinner)
    {
        TaskCompletionSource<PhysicalAddress?> task = new TaskCompletionSource<PhysicalAddress?>();
        var smartDotPage = new Window(new SmartDotsPage(spinner, task))
        {
            Title = "Connect smart dot",
            Width = 600,
            Height = 400,
            X = 100,
            Y = 100
        };
        Application.Current!.OpenWindow(smartDotPage);

        /*await MainThread.InvokeOnMainThreadAsync(() =>
        {
            Console.WriteLine("Opening SmartDotsPage");
            Application.Current!.OpenWindow(smartDotPage);
        }); This broke the smartdot connection*/

        smartDotPage.Destroying += (object? sender, EventArgs e) =>
        {
            smartDotPage = null;

            if (!task.Task.IsCompleted)
                task.SetResult(null);
        };

        var result = await task.Task;
        if (smartDotPage != null)
            Application.Current.CloseWindow(smartDotPage);

        return result;
    }

    public void InitialValues()
    {
        if (_newInitialValuesWindow != null)
            return;

        _newInitialValuesWindow = new Window(new InitialValues())
        {
            Title = "Input Values",
            Width = 400,
            Height = 500,
            X = 100,
            Y = 100
        };

        Application.Current!.OpenWindow(_newInitialValuesWindow);
        _newInitialValuesWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _newInitialValuesWindow = null;
        };
    }

    public void CloseInitialValuesWindow()
    {
        if (_newInitialValuesWindow != null)
        {
            Application.Current!.CloseWindow(_newInitialValuesWindow);
            _newInitialValuesWindow = null;
        }
    }

    public void CloudManagement()
    {
        if (_newCloudManagementWindow != null)
            return;

        _newCloudManagementWindow = new Window(new CloudManagementPage(Backend.Database))
        {
            Title = "Cloud Management"
        };

        Application.Current!.OpenWindow(_newCloudManagementWindow);
        _newCloudManagementWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _newCloudManagementWindow = null;
        };
    }

    public void Arsenal()
    {
        if (_newArsenalWindow != null)
            return;

        _newArsenalWindow = new Window(new ArsenalPage(Backend.Database))
        {
            Title = "Arsenal"
        };

        Application.Current!.OpenWindow(_newArsenalWindow);
        _newArsenalWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _newArsenalWindow = null;
        };
    }

    public void SmartDotSettings(BallSpinnerViewModel viewModel)
    {
        if (_newSmartDotSettingsWindow != null)
            return;

        _newSmartDotSettingsWindow = new Window(new SmartDotSettingsPage(this, viewModel))
        {
            Title = "Simulation Settings",
            Width = 500,
            Height = 500,
            X = 100,
            Y = 100
        };

        Application.Current!.OpenWindow(_newSmartDotSettingsWindow);
        _newSmartDotSettingsWindow.Destroying += (object? sender, EventArgs e) =>
        {
            _newSmartDotSettingsWindow = null;
        };
    }

    public void CloseSmartDotSettings()
    {
        if (_newSmartDotSettingsWindow != null)
        {
            Application.Current!.CloseWindow(_newSmartDotSettingsWindow);
            _newSmartDotSettingsWindow = null;
        }
    }
}
