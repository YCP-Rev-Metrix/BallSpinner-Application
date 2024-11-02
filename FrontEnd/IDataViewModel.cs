using Newtonsoft.Json.Linq;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// Interface for view models that will consume data from a ball spinner
/// </summary>
public interface IDataViewModel : IDisposable
{
    /// <summary>
    /// Flag collection of all metrics that the view will consume
    /// </summary>
    Metric Metrics { get; }

    /// <summary>
    /// Append a data value to 
    /// </summary>
    void OnDataReceived(Metric metric, float value, float timeFromStart);

    Action<Metric, float, float>? DataReceived { get; set; }

    /// <summary>
    /// Clear and reset all data in the view
    /// </summary>
    void Reset();
}