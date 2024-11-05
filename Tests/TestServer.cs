using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Text.Json;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

class TestServer
{
    public async Task StartServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Listening on http://localhost:8080/");

        // Route handlers
        var routeHandlers = new Dictionary<string, Func<HttpListenerRequest, object>>
        {
            { "/hello", HelloHandler },
            { "/posts/Authorize", LoginHandler }
        };

        // Listen asynchronously
        while (true)
        {
            var context = await listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            try
            {
                if (routeHandlers.TryGetValue(request.Url.AbsolutePath, out var handler))
                {
                    var responseData = handler(request);
                    JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
                    string json = JsonSerializer.Serialize(responseData, options);

                    response.ContentType = "application/json";
                    response.StatusCode = 200;
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    response.StatusCode = 404;
                    byte[] buffer = Encoding.UTF8.GetBytes("{\"error\": \"Not Found\"}");
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                byte[] buffer = Encoding.UTF8.GetBytes("{\"error\": \"" + ex.Message + "\"}");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }
    }

    // Route handler functions
    static object HelloHandler(HttpListenerRequest request)
    {
        return new { message = "Hello, world!" };
    }
    /*
     * For username = string and password = string
     */
    static object LoginHandler(HttpListenerRequest request)
    {
        // get request info
        Stream body = request.InputStream;
        Encoding encoding = request.ContentEncoding;
        StreamReader reader = new StreamReader(body, encoding);
        
        // Convert the body data to a string
        string bodyData = reader.ReadToEnd();
        // Deserialize bodyData into Credentials object
        Credentials inputCredentials = JsonSerializer.Deserialize<Credentials>(bodyData);
        
        // Declare token variables that will hold response token
        string tokenA = null;
        string tokenB = null;
        // Return specific tokens based on the input !IMPORTANT! NEED A MORE EFFICIENT WAY OF DOING THIS
        if (inputCredentials.Username == "string" && inputCredentials.Password == "string")
        {
            tokenA =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmciLCJqdGkiOiI5ZjJhN2M3My03MDQ3LTQ2ZGItODNjNC1mYWIzZjNlYzA1Y2QiLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjM4LyIsImF1ZCI6IlJldk1ldHJpeCIsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzMwNzYzOTU4LCJleHAiOjE3MzA3Njc1NTgsImlhdCI6MTczMDc2Mzk1OH0.XcB0zrR_7FsNmfzAPcdChiYTtvYhw2aXX2cfSQJCl9s";
            tokenB =
                "d8E4THTgIuPbLXxo+1bXolJxhWUz3Pr0mzdme9HemBM=";
        }
        
        return new { tokenA = tokenA, tokenB = tokenB };
    }
    /*
     * For the following registration information:
     * {
            "firstname": "string",
            "lastname": "string",
            "username": "string",
            "password": "string",
            "email": "user@example.com",
            "phoneNumber": "string"
        }
     */
    static object RegisterHandler(HttpListenerRequest request)
    {
        string tokenA =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmciLCJqdGkiOiI0NTFiZDdhNy1iMGVhLTQ3NTctYTQ0Ni1lODhhM2RkYWYxMzkiLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjM4LyIsImF1ZCI6IlJldk1ldHJpeCIsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzMwNzYzODI2LCJleHAiOjE3MzA3Njc0MjYsImlhdCI6MTczMDc2MzgyNn0.29pzZLHMKs8LUlkfXk7POOqXIkTX_2vbWdsd0cYrDe0";
        string tokenB = "fV93U2OOOtaKrQM0THhWIjkfmb+gSSgWp0F+2pn7vJE=";
        return new { tokenA = tokenA, tokenB = tokenB };
    }
}
