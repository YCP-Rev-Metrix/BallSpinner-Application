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
    /// Deletes the SimulatedShot provided by the user. Throws HttpException if response indicates failure.
    ///</Summary>
    public async Task<bool> DeleteUserShot(string? ShotName)
    {
        // If the user is not logged in, return false
        if (this.UserTokens == null)
        {
            throw new UnauthorizedAccessException("You must be logged in to access the database");
        }

        // Set the authorization header
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        // Send the delete request. Include ShotName in query parameters
        var response =
            await Client.DeleteAsync(BaseAPIURL + $"/deletes/DeleteShot?ShotName={Uri.EscapeDataString(ShotName)}");
        response.EnsureSuccessStatusCode();

        return true;
    }
}