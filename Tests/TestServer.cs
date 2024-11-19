using System;
using System.Net;
using System.Text;
using System.Text.Json;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
using JsonSerializer = System.Text.Json.JsonSerializer;

class TestServer
{
    public async Task StartServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Listening on http://localhost:8080/");

        // Route handlers
        var routeHandlers = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, object>>
        {
            { "/hello", HelloHandler },
            { "/posts/Authorize", LoginHandler },
            {"/posts/Register", RegisterHandler},
            {"/gets/GetShotsByUsername", GetShotsHandler},
            {"/posts/InsertSimulatedShot", UploadShotsHandler}
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
                    var responseData = handler(request, response);
                    JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
                    string json = JsonSerializer.Serialize(responseData, options);

                    response.ContentType = "application/json";
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
    static object HelloHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        return new { message = "Hello, world!" };
    }
    /*
     * For username = string and password = string
     */
    static object LoginHandler(HttpListenerRequest request, HttpListenerResponse response)
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
            response.StatusCode = 200;
            return new { tokenA = tokenA, tokenB = tokenB };
        }
        else 
        {
            response.StatusCode = 403;
            return new {
                          type = "string",
                          title = "string",
                          status = 0,
                          detail = "string",
                          instance = "string",
                          additionalProp1 = "string",
                          additionalProp2 = "string",
                          additionalProp3 = "string"
                        };
        }
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
    static object RegisterHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        // get request info
         Stream body = request.InputStream;
         Encoding encoding = request.ContentEncoding;
         StreamReader reader = new StreamReader(body, encoding);
                
         // Convert the body data to a string
         string bodyData = reader.ReadToEnd();
         // Deserialize bodyData into Credentials object
         User user = JsonSerializer.Deserialize<User>(bodyData);
                
         // Declare token variables that will hold response token
         string tokenA = null;
         string tokenB = null;
         // Return specific tokens based on the input !IMPORTANT! NEED A MORE EFFICIENT WAY OF DOING THIS
         if (user.username == "string" && user.password == "string")
            {
                tokenA =
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmciLCJqdGkiOiI5ZjJhN2M3My03MDQ3LTQ2ZGItODNjNC1mYWIzZjNlYzA1Y2QiLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjM4LyIsImF1ZCI6IlJldk1ldHJpeCIsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzMwNzYzOTU4LCJleHAiOjE3MzA3Njc1NTgsImlhdCI6MTczMDc2Mzk1OH0.XcB0zrR_7FsNmfzAPcdChiYTtvYhw2aXX2cfSQJCl9s";
                tokenB =
                    "d8E4THTgIuPbLXxo+1bXolJxhWUz3Pr0mzdme9HemBM=";
                response.StatusCode = 200;
                return new { tokenA = tokenA, tokenB = tokenB };
            }
         if (user.username == "403Response" && user.password == "403Response")
             {
                response.StatusCode = 403;
                return new {
                              type = "string",
                              title = "string",
                              status = 0,
                              detail = "string",
                              instance = "string",
                              additionalProp1 = "string",
                              additionalProp2 = "string",
                              additionalProp3 = "string"
                            };
                }
             return null;
    } //Still needs to be implemented
    static object GetShotsHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        // Get jwt token from header
        string auth = request.Headers["Authorization"];
        
        string JWT = auth.Substring("Bearer ".Length).Trim();

        if (JWT  == "403")
        {
            response.StatusCode = 403;
            return null;
        }
        
        // TODO - Test with fake SimulatedShot list return data

        return null;
    }

    static object UploadShotsHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        // Get jwt token from header
        string auth = request.Headers["Authorization"];

        string JWT = auth.Substring("Bearer ".Length).Trim();

        if (JWT == "403")
        {
            response.StatusCode = 403;
            return null;
        }

        return null;
    }
}
