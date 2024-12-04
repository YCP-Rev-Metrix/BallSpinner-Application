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
    /// Uploads the metadata for a locally saved shot to the database for future indexing.
    /// Throws exception on Http error. In the future, this will take/provide more detailed metadata.
    /// Throws HttpException if response indicates failure.
    ///</Summary>
    public async Task<bool> SaveLocalEntry(string ShotName)
    {
        var RequestData = new LocalShot();
        RequestData.ShotName = ShotName;
        // Set the authorization header
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        
        var jsonBody = JsonConvert.SerializeObject(RequestData);

        // Create the request content
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(BaseAPIURL + "/posts/SaveLocalShot", content);
        // Ensure success. This will throw an exception on fail.
        response.EnsureSuccessStatusCode();
        
        return true;
    }
}