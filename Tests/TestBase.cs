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
        revFilePath = Utilities.GetTempRevFileDir("Tests");
        TempFileWriter = new WriteToTempRevFile(revFilePath);
        FrontEnd.Init(BackEnd);
        BackEnd.Init(FrontEnd, Database);
    }

    protected virtual void Cleanup()
    {
        TempFileWriter.Dispose();
    }

    ~TestBase()
    {
        Cleanup();
    }
}