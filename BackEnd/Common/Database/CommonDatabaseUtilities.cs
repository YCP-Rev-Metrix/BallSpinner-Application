using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    private static readonly HttpClient Client = new HttpClient();
    private DatabaseTypes dbType;
    private string BaseAPIURL;
    public Token UserTokens;

    /// <inheritdoc/>
    public event Action<bool>? OnLoginChanged;

    public Database(DatabaseTypes dbType)
    {
        this.dbType = dbType;
        if (this.dbType == DatabaseTypes.FAKEDATABASE)
        {
            BaseAPIURL = "http://localhost:8080";
        }
        else if (this.dbType == DatabaseTypes.REALDATABASE)
        {
            BaseAPIURL = "https://api.revmetrix.io/api";
        }
    }

    public void SetUserTokens(Token token)
    {
        this.UserTokens = token;
    }
}