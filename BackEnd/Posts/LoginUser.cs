using System.Text;
using Newtonsoft.Json;
using Common.POCOs;
namespace RevMetrix.BallSpinner.BackEnd.Database;

///<Summary>
/// Placeholder (fill in this section later)
///</Summary>
public partial class Database: IDatabase
{
    ///<Summary>
    /// Database method used to log a user in. On success, OnLoginChanged event is invoked, session tokens are set, and
    /// the 'CurrentUser' environment variable is set as well.
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
        
        // Set the users token for the session
        SetUserTokens(responseObject);
        OnLoginChanged?.Invoke(UserTokens is not null);
        
        // On success, set global Username property to be used in the future
        Environment.SetEnvironmentVariable("CurrentUser", username);
        
        return responseObject;
    }
}
