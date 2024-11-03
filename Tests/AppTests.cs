using RevMetrix.BallSpinner.BackEnd.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.Tests;
public class AppTests : TestBase
{
    [Fact]
    private void InitializesTest()
    {
        Assert.NotNull(BackEnd);
        Assert.NotNull(Database);
        Assert.NotNull(FrontEnd);
        Assert.NotNull(BackEnd.Database);
        Assert.NotNull(BackEnd.FrontEnd);
    }
}
