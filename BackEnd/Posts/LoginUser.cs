using System.Text;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd;
namespace BackEnd.Posts
{
    public partial class Database: IDatabase
    {
        private static readonly HttpClient Client = new HttpClient();
        
        public class AuthenticateResponse
        {
            public string? TokenA { get; set; }
            public string? TokenB { get; set; }
        }

        private class RequestUserAuthenticate
        {
            public string? firstName { get; set; }
            public string? lastName { get; set; }
            public string? username { get; set; }
            public string? password { get; set; }
            public string? email { get; set; }
            public string? phone_number { get; set; }
        }

        public async Task LoginUser()
        {
            var request = new RequestUserAuthenticate()
            {
                firstName = "string",
                lastName = "string",
                username = "string",
                password = "string",
                email = "string",
                phone_number = "string"
            };

            var jsonBody = JsonConvert.SerializeObject(request);

            // Create the request content
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            try
            {
                var response = await Client.PostAsync("https://api.revmetrix.io/api/posts/Authorize", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                var responseObject = JsonConvert.DeserializeObject<AuthenticateResponse>(responseBody);
                
                // Output the tokens
                Console.WriteLine($"TokenA: {responseObject?.TokenA}");
                Console.WriteLine($"TokenB: {responseObject?.TokenB}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
            }
            catch (JsonException je)
            {
                Console.WriteLine("JSON parsing error: " + je.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
