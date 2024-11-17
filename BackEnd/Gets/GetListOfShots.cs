using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Retrives a set of user shots
    ///</Summary>
    public async Task<SimulatedShotList?> GetListOfShots()
    {
        // If the user is not logged in, return false
        if (this.UserTokens.TokenA == null)
        {
            throw new UnauthorizedAccessException();
        }

        // Set the authorization header
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        var response = await Client.GetAsync(BaseAPIURL + "/gets/GetShotsByUsername");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        // Parse the JSON response
        var responseObject = JsonConvert.DeserializeObject<SimulatedShotList>(responseBody);

        return responseObject;
    }
}