using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd;
using Common.POCOs;
namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Deletes the bowling ball provided by the user. Throws HttpException if response indicates failure.
    ///</Summary>
    public async Task<bool> DeleteBowlingBall(string BallName)
    {
        // If the user is not logged in, return false
        if (this.UserTokens == null)
        {
            throw new UnauthorizedAccessException("You must be logged in to access the database");
        }

        // Set the authorization header
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        // Send the delete request. Include BallName in query parameters
        var response =
            await Client.DeleteAsync(BaseAPIURL + $"/deletes/DeleteBall?ballName={Uri.EscapeDataString(BallName)}");
        response.EnsureSuccessStatusCode();

        return true;
    }
}