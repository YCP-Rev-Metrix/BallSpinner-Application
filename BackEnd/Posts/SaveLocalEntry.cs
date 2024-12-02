using System.Text;
using System.Xml;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd;


namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Uploads the metadata for a locally saved shot to the database for future indexing.
    /// Throws exception on Http error. In the future, this will take/provide more detailed metadata.
    /// FOR THE TIME BEING THIS WILL NOT WORK WITH THE DATABASE!
    ///</Summary>
    public async Task<bool> SaveLocalEntry(string ShotName)
    {
        var RequestData = new 
        {
            ShotName = ShotName   
        };
        var jsonBody = JsonConvert.SerializeObject(RequestData);

        // Create the request content
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(BaseAPIURL + "/posts/SetLocalShot", content);
        // Ensure success. This will throw an exception on fail.
        response.EnsureSuccessStatusCode();
        
        return true;
    }
}