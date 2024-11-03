using System.Text;
using System.Xml;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Placeholder (fill in this section later)
    ///</Summary>
    public async Task<Token?> RegisterUser(string firstname, string lastname, string username, string password,
        string email, string phonenumber)
    {
        User user = new User(firstname, lastname, username, password, email, phonenumber);

        var jsonBody = JsonConvert.SerializeObject(user);

        // Create the request content
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("https://api.revmetrix.io/api/posts/Register", content);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();

        // Parse the JSON response
        var responseObject = JsonConvert.DeserializeObject<Token>(responseBody);

        return responseObject;
    }
}
