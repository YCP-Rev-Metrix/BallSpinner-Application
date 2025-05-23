using System.Text;
using System.Xml;
using Common.POCOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RevMetrix.BallSpinner.BackEnd;

namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Database method used to register a user. On success, OnLoginChanged event is invoked, session tokens are set, and
    /// the 'CurrentUser' environment variable is set as well. Also logs in a user on success.
    ///</Summary>
    public async Task<Token?> RegisterUser(string firstname, string lastname, string username, string password,
        string email, string phonenumber)
    {
        UserIdentification user = new UserIdentification(firstname, lastname, username, password, email, phonenumber);
        // Create settings to preserve PascalCase
        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver() // Keeps original casing (PascalCase)
        };

        var jsonBody = JsonConvert.SerializeObject(user, jsonSettings);

        // Create the request content
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(BaseAPIURL + "/posts/Register", content);
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
