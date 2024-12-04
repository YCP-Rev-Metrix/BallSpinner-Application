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
    /// Retrieves the input values that were given to a shot, for the user selected shot.
    /// Throws HttpException if response indicates failure.
    ///</Summary>
    public async Task<ShotInfo?> GetInitialValuesForShot(string shotName)
    {
        // If the user is not logged in, return false
        if (this.UserTokens == null)
        {
            throw new UnauthorizedAccessException("You must be logged in to access the database");
        }

        // Set the authorization header
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        var response = await Client.GetAsync(BaseAPIURL + $"/gets/GetInitialValuesForShot?shotName={Uri.EscapeDataString(shotName)}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        // Parse the JSON response
        var responseObject = JsonConvert.DeserializeObject<ShotInfo>(responseBody);

        return responseObject;
    }
}