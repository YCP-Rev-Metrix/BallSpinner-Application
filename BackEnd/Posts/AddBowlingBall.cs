using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Common.POCOs;

namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Uploades a new bowling ball to the database for the user that is currently logged in.
    /// Throws HttpException if response indicates failure.
    ///</Summary>
    public async Task<bool> AddBowlingBall(Ball ball)
    {
        // User is not logged in. Return false
        if (this.UserTokens == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var jsonBody = JsonConvert.SerializeObject(ball);
        // Create the request content
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        var response = await Client.PostAsync(BaseAPIURL + "/posts/InsertBall", content);
        response.EnsureSuccessStatusCode();

        return true;
    }
}