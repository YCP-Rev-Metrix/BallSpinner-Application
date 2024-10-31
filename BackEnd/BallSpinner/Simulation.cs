using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;
/// <summary>
/// Interface defining the structure for simulation components in the backend.
/// </summary>
public class Simulation
{

    /// <summary>
    ///Gets or sets the name of the Ball Spinner device.
    /// </summary>
    string Name { get; set; }


    /// <summary>
    /// 
    /// Disposes of resources used by the simulation component.
    /// </summary>
    public void Dispose() { }
    

}
