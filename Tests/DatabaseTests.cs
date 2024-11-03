using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.Tests;
public class DatabaseTests : TestBase
{
    [Fact]
    private void InitializesTest()
    {
        Assert.NotNull(Database);
    }
}
