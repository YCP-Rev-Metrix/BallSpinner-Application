using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Database;
using Xunit.Abstractions;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;

namespace RevMetrix.BallSpinner.Tests;
// Initializes all resources needed by the tests before the tests start
public abstract class TestBase
{
    public IFrontEnd FrontEnd = null!;
    public Backend BackEnd = null!;
    public IDatabase Database = null!;
    public WriteToTempRevFile TempFileWriter = null!;
    public string revFilePath = null!;
    public double epsilon = 0.01;

    protected TestBase()
    {
        Init();
    }

    protected virtual void Init()
    {
        FrontEnd = new MockFrontEnd();
        BackEnd = new Backend();
        
        Database = new Database(DatabaseTypes.FAKEDATABASE);
        Token token = new Token
        {
            TokenA = "test",
            TokenB = "test"
        };
        Database.SetUserTokens(token);
        revFilePath = "Testing";
        TempFileWriter = new WriteToTempRevFile(revFilePath);
        TempFileWriter.OnRecordAdded += HandleRecordAdded;
        FrontEnd.Init(BackEnd);
        BackEnd.Init(FrontEnd, Database);
    }

    protected virtual void Cleanup()
    {
        TempFileWriter.Dispose();
    }

    private void HandleRecordAdded()
    {

    }

    ~TestBase()
    {
        Cleanup();
    }
}