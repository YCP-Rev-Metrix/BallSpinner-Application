using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using Common.POCOs.Shots;

namespace RevMetrix.BallSpinner.BackEnd.Database;

public partial class Database : IDatabase
{
    ///<Summary>
    /// Uploades a user's shot from the local rev file after running a simulated shot. Returns status of HTTP request.
    /// For now, we are just passing InitialSpeed and name as an argument because that is the only ones we have.
    /// Throws and exception if the Temp csv file is empty and/or user is not logged in. Also Throws HttpException
    /// if response indicates failure.
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
        //FOR NOW THIS IS ALL DUMMY DATA TO TEST THESE ENDPOINTS. NEED TO INTEGRATE WITH FRONTEND TO ENABLE THESE PARAMETERS TO BE SET BY THE USERA
        Coordinate initpoint = new Coordinate(1.2, 1.1);
        Coordinate inflection = new Coordinate(1.2, 1.1);
        Coordinate finalpoint = new Coordinate(1.2, 1.1);

        ShotInfo parameters = new ShotInfo(name, initpoint, inflection, finalpoint, 0.003);
        Ball ball = ballSpinner.ball;

        byte[] macaddy = Convert.FromHexString("001A2B3C4D5E");

        SmartDotInfo sensorInfo = new SmartDotInfo
        {
            MACAddress = macaddy,
            Comments = "This was a very good module to use. SO GOOOOOOODDD!!!!",
        };
        var requestObject = new
        {
            shotinfo = parameters,
            data = sampleData,
            ball = ball,
            sensorInfo = sensorInfo
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
    /// Database utility method that parses temp rev file and puts data into a SampleData list
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
