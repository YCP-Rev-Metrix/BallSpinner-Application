using System.Text;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
namespace RevMetrix.BallSpinner.BackEnd.Database;

///<Summary>
/// Placeholder (fill in this section later)
///</Summary>
public partial class Database: IDatabase
{
    ///<Summary>
    /// Placeholder (fill in this section later)
    ///</Summary>
    public async Task<Token?> LoginUser(string username, string password)
    {
        Credentials credentials = new Credentials(username, password);
        var jsonBody = JsonConvert.SerializeObject(credentials);

        // Create the request content
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(BaseAPIURL + "/posts/Authorize", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        // Parse the JSON response
        var responseObject = JsonConvert.DeserializeObject<Token>(responseBody);
        
        return responseObject;
    }
}
