using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RevMetrix.BallSpinner.BackEnd.Database;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;
using Xunit;

namespace RevMetrix.BallSpinner.Tests;
public class DatabaseTests : TestBase
{
    private static readonly HttpClient Client = new HttpClient();
    // Constructor to do initialization before tests run
    public DatabaseTests() 
    {
        // Initialze test server used to mimick database API endpoints
        // need to figure out how to write thread this,
        TestServer server = new TestServer();
        server.StartServer(); // does not have await because this will never return. Need a solution for this
    }
    
    [Fact]
    private async void TestMockServer() 
    { 
        // Create the request content
        var response = await Client.PostAsync("http://localhost:8080/hello", null);
        response.EnsureSuccessStatusCode();
                
        var responseBody = await response.Content.ReadAsStringAsync();
        
        // Parse the JSON response
        dynamic responseObject = JsonConvert.DeserializeObject(responseBody);
        Console.WriteLine(responseObject);
        
        string expected = "Hello, world!";
        string actual = responseObject.message;
        
        Assert.Equal(actual, expected);
    }

    [Fact]
    private async void SuccessfulLoginTests()
    {

        var token = await Database.LoginUser("string", "string");

        string tokenA = token.TokenA;
        string tokenB = token.TokenB;

        string expectedTokenA =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmciLCJqdGkiOiI5ZjJhN2M3My03MDQ3LTQ2ZGItODNjNC1mYWIzZjNlYzA1Y2QiLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjM4LyIsImF1ZCI6IlJldk1ldHJpeCIsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzMwNzYzOTU4LCJleHAiOjE3MzA3Njc1NTgsImlhdCI6MTczMDc2Mzk1OH0.XcB0zrR_7FsNmfzAPcdChiYTtvYhw2aXX2cfSQJCl9s";
        string expectedTokenB = "d8E4THTgIuPbLXxo+1bXolJxhWUz3Pr0mzdme9HemBM=";

        Assert.NotNull(token);
        Assert.Equal(tokenA, expectedTokenA);
        Assert.Equal(tokenB, expectedTokenB);
    }
    [Fact]
    private async void UnsuccessfulLoginTests()
    {
          // Ensure method thorws exception when server responds with 403
          await Assert.ThrowsAsync<HttpRequestException>(() => Database.LoginUser("incorrect", "incorrect"));
    }
    [Fact]
    private async void SuccessfulRegisterTests()
    {
            // successful register example
            var token = await Database.RegisterUser("string", "string", "string", "string", "string", "string");
    
            string tokenA = token.TokenA;
            string tokenB = token.TokenB;
    
            string expectedTokenA =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmciLCJqdGkiOiI5ZjJhN2M3My03MDQ3LTQ2ZGItODNjNC1mYWIzZjNlYzA1Y2QiLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjM4LyIsImF1ZCI6IlJldk1ldHJpeCIsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzMwNzYzOTU4LCJleHAiOjE3MzA3Njc1NTgsImlhdCI6MTczMDc2Mzk1OH0.XcB0zrR_7FsNmfzAPcdChiYTtvYhw2aXX2cfSQJCl9s";
            string expectedTokenB = "d8E4THTgIuPbLXxo+1bXolJxhWUz3Pr0mzdme9HemBM=";
    
            Assert.NotNull(token);
            Assert.Equal(tokenA, expectedTokenA);
            Assert.Equal(tokenB, expectedTokenB);
    }
    
    [Fact]
    private async void UnsuccessfulRegisterTests()
    {
          // Ensure method thorws exception when server responds with 403
          await Assert.ThrowsAsync<HttpRequestException>(() => Database.RegisterUser("string", "string", "403Response", "403Response", "string", "string"));
    }
    
    [Fact]
    private async void GetSampleDataTests()
    {
        string path = Utilities.GetTempDir() + "/TestTempRev.csv";
        string[] dataArray = new string[6] 
        {
            "Gyroscope", "5", "43", "434.212", "4342.2", "23423"
        };
        
        TempFileWriter.WriteData(dataArray);
        
        string[] dataArray2 =
        {
            "Accelerometer", "52", "4", "112.4124", "4342412.2", "44"
        };
                
        TempFileWriter.WriteData(dataArray2);

        string[] dataArray3 =
        {
            "Magnetometer", "532", "4323", "2112.4124", "431112.2", "765.2"
        };
        
        TempFileWriter.WriteData(dataArray3);

        List<SampleData> sample = new List<SampleData>();
        await Database.GetSampleData(sample, path);
        // Test the contents of sample to make sure it is parsed into JSON correctly
        //Assert.Empty(sample);
        // Test to make sure dataArray1 is parsed correctly into sample and is 1st element
        Assert.Equal("Gyroscope", sample[0].type);
        Assert.Equal(5, sample[0].count);
        Assert.Equal(43, sample[0].logtime);
        Assert.Equal((float) 434.212, sample[0].X);
        Assert.Equal((float) 4342.2, sample[0].Y);
        Assert.Equal((float)23423, sample[0].Z);
        // Test to make sure dataArray2 is parsed correctly into sample and is 2nd element
        Assert.Equal("Accelerometer", sample[1].type);
        Assert.Equal(52, sample[1].count);
        Assert.Equal(4, sample[1].logtime);
        Assert.Equal((float) 112.4124, sample[1].X);
        Assert.Equal((float) 4342412.2, sample[1].Y);
        Assert.Equal((float)44, sample[1].Z);
        // Test to make sure dataArray3 is parsed correctly into sample and is 3nd element
        Assert.Equal("Magnetometer", sample[2].type);
        Assert.Equal(532, sample[2].count);
        Assert.Equal(4323, sample[2].logtime);
        Assert.Equal((float) 2112.4124, sample[2].X);
        Assert.Equal((float) 431112.2, sample[2].Y);
        Assert.Equal((float)765.2, sample[2].Z);
    }
}
