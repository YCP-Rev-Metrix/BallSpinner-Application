using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.BackEnd;

/// <summary>
/// Limited interface for bridging from Back end to Front end.
/// </summary>
public interface IFrontEnd : IDisposable
{
    /// <summary>
    /// Initializes the front end.
    /// Does not use constructor because the front end and back end both depend on each other to get an initial object reference.
    /// </summary>
    void Init(Backend backend);
}
