using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using CsvHelper;
using CsvHelper.Configuration;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Uploades a user's shot from the local rev file after running a simulated shot. Returns status of HTTP request.
    /// For now, we are just passing InitialSpeed and name as an argument because that is the only ones we have.
    /// Throws and exception if the Temp csv file is empty and/or user is not logged in.
    ///</Summary>
    public async Task<bool> UploadShot(IBallSpinner ballSpinner, string name, float InitialSpeed)
    {
        // User is not logged in. Return false
        if (this.UserTokens == null)
        {
            throw new UnauthorizedAccessException();
        }

        List<SampleData> sampleData = new List<SampleData>();
        // Get sample data from temp rev file
        await GetSampleData(sampleData, ballSpinner.DataParser.TempFilePath);
        //If the csv is empty...
        if (sampleData.Count == 0)
        {
            throw new Exception("No data to upload. Temporary csv is empty.");
        }

        ShotInfo parameters = new ShotInfo(name, InitialSpeed, 20, 20, 20);
        var requestObject = new
        {
            simulatedShot = parameters,
            data = sampleData
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
    ///<Summary>
    /// Parses temp rev file and puts data into a SampleData list
    ///</Summary>
    public async Task<List<SampleData>> GetSampleData(List<SampleData> sampleData, string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord  = false,
        };
        
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {
            while (csv.Read())
            {
                var record = csv.GetRecord<SampleData>();
                sampleData.Add(record);
            }
        }
        return sampleData;
    }
}
