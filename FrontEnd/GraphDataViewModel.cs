using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// Base class of data view models that are backed a <see cref="WebView"/>
/// </summary>
public abstract class GraphDataViewModel : IDataViewModel
{
    public string Name { get; }

    public abstract Metric Metrics { get; }
    public Action<Metric, float, float>? DataReceived { get; set; }

    public Action<double, bool> InitializeSimulation { get; set; }

    private IBallSpinner _ballSpinner;

    public GraphDataViewModel(IBallSpinner ballSpinner)
    {
        _ballSpinner = ballSpinner;
        _ballSpinner.DataParser.Subscribe(OnDataReceived);
    }

    public void Dispose()
    {
        _ballSpinner.DataParser.Unsubscribe(OnDataReceived);
    }

    public void Reset()
    {
        //_webView.Reload();
    }

    public void OnDataReceived(Metric metric, float value, float timeFromStart)
    {
        DataReceived?.Invoke(metric, value, timeFromStart);
    }
}
