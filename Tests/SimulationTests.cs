using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.Tests;
public class SimulationTests : TestBase
{
    [Fact]
    private void SimulationInitsTest()
    {
        var simulation = new Simulation(1);
        simulation.Dispose();
    }
}
