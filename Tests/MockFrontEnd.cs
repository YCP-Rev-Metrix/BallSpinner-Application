using RevMetrix.BallSpinner.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.Tests;

internal class MockFrontEnd : IFrontEnd
{
    public void Alert(string message)
    {
        //Do nothing
    }

    public void Dispose()
    {

    }

    public void Init(Backend backend)
    {
        
    }
}
