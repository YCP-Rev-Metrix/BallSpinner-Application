using System.Collections;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd;
using CsvHelper;
using CsvHelper.Configuration;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Uploades a user's shot from the local rev file after running a simulated shot. Returns status of HTTP request.
    /// For now, we are just passing InitialSpeed and name as an argument because that is the only ones we have.
    ///</Summary>
    public async Task<bool> UploadShot(string name, float InitialSpeed)
    {
        // User is not logged in. Return false
        if (this.UserTokens.TokenA == null)
        {
            return false;
        }
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord  = false,
        };

        List<SampleData> sampleData = new List<SampleData>();
        
        using (var reader = new StreamReader("./Backend/BallSpinner/TempRev.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Read();
            while (csv.Read())
            {
                var record = csv.GetRecord<SampleData>();
                sampleData.Add(record);
            }
        }

        ShotInfo parameters = new ShotInfo(name, InitialSpeed, null, null, null);
        var requestObject = new
        {
            parameters = parameters,
            sampleData = sampleData
        };
        var jsonBody = JsonConvert.SerializeObject(requestObject);
        // Create the request content
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", this.UserTokens.TokenA);
        var response = await Client.PostAsync(BaseAPIURL + "/posts/InsertSimulatedShot", content);
        response.EnsureSuccessStatusCode();
        
        return true;
    }
}
