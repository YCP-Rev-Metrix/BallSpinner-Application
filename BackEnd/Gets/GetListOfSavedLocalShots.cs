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
    /// Retrieves a list containing the user's local shots. Throws HttpException if response indicates failure.
    ///</Summary>
    public async Task<LocalShotList?> GetListOfSavedLocalShots()
    {
        // If the user is not logged in, return false
        if (this.UserTokens == null)
        {
            throw new UnauthorizedAccessException("You must be logged in to access the database");
        }

        // Set the authorization header
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        
        var response = await Client.GetAsync(BaseAPIURL + "/gets/GetLocalShots");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        // Parse the JSON response
        var responseObject = JsonConvert.DeserializeObject<LocalShotList>(responseBody);

        return responseObject;
    }
}