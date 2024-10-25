using System.Text;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.BackEnd.Database
{
    public partial class Database : IDatabase
    {
        ///<Summary>
        /// Placeholder (fill in this section later)
        ///</Summary>
        public async Task Register()
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