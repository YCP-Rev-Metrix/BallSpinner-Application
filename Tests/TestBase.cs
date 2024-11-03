using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Database;
using Xunit.Abstractions;

namespace RevMetrix.BallSpinner.Tests;

public abstract class TestBase
{
    public IFrontEnd FrontEnd = null!;
    public Backend BackEnd = null!;
    public IDatabase Database = null!;

    protected TestBase()
    {
        Init();
    }

    protected virtual void Init()
    {
        FrontEnd = new MockFrontEnd();
        BackEnd = new Backend();

        Database = new MockDatabase();

        FrontEnd.Init(BackEnd);
        BackEnd.Init(FrontEnd, Database);
    }

    protected virtual void Cleanup()
    {
    }

    ~TestBase()
    {
        Cleanup();
    }
}