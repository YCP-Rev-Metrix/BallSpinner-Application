using Newtonsoft.Json;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using Common.POCOs;


namespace RevMetrix.BallSpinner.Tests;
// Refer to TestServer class to see documentation regarding the different token types
// that can be used to invoke certain responses
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
    private async void UploadShotTests()
    {
        // Test how the database method reacts to a 403
        Token token = new Token()
        {
            TokenA = "403",
            TokenB = "403"
        };
        Database.SetUserTokens(token);

        var ballSpinner = new Simulation(1);
        ballSpinner.Start();
        Thread.Sleep(1000);
        ballSpinner.Stop();

        await Assert.ThrowsAsync<HttpRequestException>(() => Database.UploadShot(ballSpinner, "Test", 20));

        ballSpinner.Dispose();
    }

    [Fact]
    private async void GetSampleDataTests()
    {
        string[] dataArray = new string[6] 
        {
            "2", "5.0", "43", "434.212", "4342.2", "23423"
        };

        TempFileWriter.Start();
        TempFileWriter.WriteData(dataArray);
        
        string[] dataArray2 =
        {
            "3", "52.0", "4", "112.4124", "4342412.2", "44"
        };
                
        TempFileWriter.WriteData(dataArray2);

        string[] dataArray3 =
        {
            "4", "532.0", "4323", "2112.4124", "431112.2", "765.2"
        };
        
        TempFileWriter.WriteData(dataArray3);

        List<SampleData> sample = new List<SampleData>();
        await Database.GetSampleData(sample, revFilePath, 3, DataParser.NUM_DATA_POINTS);
        // Dispose of memory mapped file
        TempFileWriter.Stop();
        // Test the contents of sample to make sure it is parsed into JSON correctly
        //Assert.Empty(sample);
        // Test to make sure dataArray1 is parsed correctly into sample and is 1st element
        Assert.Equal("2", sample[0].Type);
        Assert.Equal(43, sample[0].Count);
        Assert.Equal(5.0, sample[0].Logtime);
        Assert.InRange((double) sample[0].X, 434.212 - epsilon, 434.212 + epsilon);
        Assert.InRange((double)sample[0].Y, 4342.2 - epsilon, 4342.2 + epsilon);
        Assert.InRange((double)sample[0].Z, 23423 - epsilon, 23423 + epsilon);
        // Test to make sure dataArray2 is parsed correctly into sample and is 2nd element
        Assert.Equal("3", sample[1].Type);
        Assert.Equal(4, sample[1].Count);
        Assert.Equal(52.0, sample[1].Logtime);
        Assert.InRange((double)sample[1].X, 112.4124 - epsilon, 112.4124 + epsilon);
        Assert.InRange((double)sample[1].Y, 4342412.2 - epsilon, 4342412.2 + epsilon);
        Assert.InRange((double)sample[1].Z, 44 - epsilon, 44 + epsilon);
        // Test to make sure dataArray3 is parsed correctly into sample and is 3nd element
        Assert.Equal("4", sample[2].Type);
        Assert.Equal(4323, sample[2].Count);
        Assert.Equal(532.0, sample[2].Logtime);
        Assert.InRange((double)sample[2].X, 2112.4124 - epsilon, 2112.4124 + epsilon);
        Assert.InRange((double)sample[2].Y, 431112.2 - epsilon, 431112.2 + epsilon);
        Assert.InRange((double)sample[2].Z, 765.2 - epsilon, 765.2 + epsilon);
    }

    [Fact]
    private async void GetShotsTest()
    {
        // Test how the database method reacts to a 403
        Token token = new Token()
        {
            TokenA = "403",
            TokenB = "403"
        };
        Database.SetUserTokens(token);
        await Assert.ThrowsAsync<HttpRequestException>(() => Database.GetListOfShots());
    }

    [Fact]
    private async void DeleteBowlingBallTests()
    {
        // Setup token
        Token token = new Token();
        token.TokenA = "jiowdenjewn";
        token.TokenB = "iojewfownf";
        
        // Test to see if DeleteBowlingBall throws unauthoirzed exception if user is not logged in
        Database.SetUserTokens(null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Database.DeleteBowlingBall("test"));
        // Make sure the method indicates success
        Database.SetUserTokens(token);
        bool completed = await Database.DeleteBowlingBall("test");
        Assert.True(completed);
        // See how method responds to http error
        token.TokenA = "403";
        token.TokenB = "403";
        Database.SetUserTokens(token);
        Assert.ThrowsAsync<HttpRequestException>(() => Database.DeleteBowlingBall("test"));

    }
    
    [Fact]
    private async void DeleteShotTests()
    {
        // Setup token
        Token token = new Token();
        token.TokenA = "jiowdenjewn";
        token.TokenB = "iojewfownf";
        
        // Test to see if DeleteBowlingBall throws unauthoirzed exception if user is not logged in
        Database.SetUserTokens(null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Database.DeleteUserShot("test"));
        // Make sure the method indicates success
        Database.SetUserTokens(token);
        bool completed = await Database.DeleteUserShot("test");
        Assert.True(completed);
        // See how method responds to http error
        token.TokenA = "403";
        token.TokenB = "403";
        Database.SetUserTokens(token);
        Assert.ThrowsAsync<HttpRequestException>(() => Database.DeleteUserShot("test"));
    }
    
    [Fact]
    private async void GetArsenalTests()
    {
        // Test to see if DeleteBowlingBall throws unauthoirzed exception if user is not logged in
        Database.SetUserTokens(null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Database.GetArsenal());
        // Setup token
        Token token = new Token();
        token.TokenA = "jiowdenjewn";
        token.TokenB = "iojewfownf";
        Database.SetUserTokens(token);
        Arsenal balls = await Database.GetArsenal();
        
        Assert.Equal(balls.BallList[0].Name, "Test");
        Assert.Equal(balls.BallList[0].Diameter, 20.2);
        Assert.Equal(balls.BallList[0].Weight, 11.3);
        Assert.Equal(balls.BallList[0].CoreType, "Pancake");
        
        token.TokenA = "empty";
        token.TokenB = "empty";
        Database.SetUserTokens(token);
        balls = await Database.GetArsenal();
        Assert.Empty(balls.BallList);

        token.TokenA = "403";
        token.TokenB = "403";
        Database.SetUserTokens(token);
        Assert.ThrowsAsync<HttpRequestException>(() => Database.GetArsenal());

    }
    
    [Fact]
    private async void InsertBallTests()
    {
        Token token = new Token();
        token.TokenA = "ewknflwk";
        token.TokenB = "ewknflwk";
        // Setup ball to add
        Ball ball = new Ball("Test", 20.2, 10.5, "Pancake");
        // Test to see if AddBowlingBall throws unauthoirzed exception if user is not logged in
        Database.SetUserTokens(null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Database.AddBowlingBall(ball));
        // Make sure the method indicates success
        Database.SetUserTokens(token);
        bool completed = await Database.AddBowlingBall(ball);
        Assert.True(completed);
        // See how method responds to http error
        token.TokenA = "403";
        token.TokenB = "403";
        Database.SetUserTokens(token);
        Assert.ThrowsAsync<HttpRequestException>(() => Database.DeleteUserShot("test"));
    }
}
