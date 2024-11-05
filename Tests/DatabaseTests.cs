using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RevMetrix.BallSpinner.BackEnd.Database;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
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
    private async void LoginTests()
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
    
}
