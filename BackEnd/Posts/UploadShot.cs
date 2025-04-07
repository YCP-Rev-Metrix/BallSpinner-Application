using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.IO.MemoryMappedFiles;
using System.Collections;
using System.Globalization;
using System.Diagnostics;

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
        await GetSampleData(sampleData, ballSpinner.DataParser.TempFilePath, ballSpinner.DataParser.NumRecords, DataParser.NUM_DATA_POINTS);
        //If the csv is empty...
        if (sampleData.Count == 0)
        {
            throw new Exception("No data to upload. Temporary csv is empty.");
        }

        ShotInfo parameters = new ShotInfo
        {
            Name = name,
            BezierInitPoint = ballSpinner.BezierInitPoint,
            BezierInflectionPoint = ballSpinner.BezierInflectionPoint,
            BezierFinalPoint = ballSpinner.BezierFinalPoint,
            TimeStep = 0.010,
            Comments = ballSpinner.Comments
        };
        Ball ball = ballSpinner.ball;

        SimulatedShot shot = new SimulatedShot
        {
            shotinfo = parameters,
            data = sampleData,
            ball = ball,
        };
        var jsonBody = JsonConvert.SerializeObject(shot);
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
    public async Task<List<SampleData>> GetSampleData(List<SampleData> sampleData, string path, int numRecords, int numDataPoints)
    {
        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(path))
        {
            int position = 0;
            List<byte[]> records = new List<byte[]>();
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                for (int i = 0; i < numRecords; i++)
                {
                    if (i == numRecords - 1)
                    {
                        Debug.WriteLine("Limit reached");
                    }
                    try
                    {
                        for (int j = 1; j <= numDataPoints; j++)
                        {
                            int length = accessor.ReadInt32(position); // Read length first
                            position += 4; // Read length which is 4 bytes, now move forward 4 bytes
                            byte[] dataPoints = new byte[length];
                            accessor.ReadArray(position, dataPoints, 0, length); // Offset by 4 bytes
                            records.Add(dataPoints);

                            position += length;
                            // If an entire sample point has been read
                            if (j == numDataPoints)
                            {
                                // Records are in order
                                float.Parse(Encoding.UTF8.GetString(records[0]));
                                SampleData data = new SampleData()
                                {
                                    Type = Encoding.UTF8.GetString(records[0]),
                                    Logtime = double.Parse(Encoding.UTF8.GetString(records[1]), CultureInfo.InvariantCulture),
                                    Count = int.Parse(Encoding.UTF8.GetString(records[2]), CultureInfo.InvariantCulture),
                                    X = double.Parse(Encoding.UTF8.GetString(records[3]), CultureInfo.InvariantCulture),
                                    Y = double.Parse(Encoding.UTF8.GetString(records[4]), CultureInfo.InvariantCulture),
                                    Z = double.Parse(Encoding.UTF8.GetString(records[5]), CultureInfo.InvariantCulture),
                                };
                                sampleData.Add(data);
                                records.Clear();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(i);
                    }
                }
            }
        }
        return sampleData;
    }
}
