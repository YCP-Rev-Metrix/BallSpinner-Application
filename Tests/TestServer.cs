using System;
using System.Net;
using System.Text;
using System.Text.Json;
using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd;
using JsonSerializer = System.Text.Json.JsonSerializer;

class TestServer
/* Token Types - Refers to the values of a JWT token that can be set by the test cases to invoke
    * certain responses from the testserver
    *      "403" - Return a 403 response
    *      "empty" - Return an ampty response object
    */
{
    private HttpListener listener;
    public async Task StartServer()
    {
        listener = new HttpListener();
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
            {"/posts/InsertSimulatedShot", UploadShotsHandler},
            {"/deletes/DeleteBall", DeleteBallHandler},
            {"/deletes/DeleteShot", DeleteShotHandler},
            {"/gets/GetArsenal", GetArsenalHandler},
            {"/posts/InsertBall", InsertBallHandler},
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
         UserIdentification user = JsonSerializer.Deserialize<UserIdentification>(bodyData);
                
         // Declare token variables that will hold response token
         string tokenA = null;
         string tokenB = null;
         // Return specific tokens based on the input !IMPORTANT! NEED A MORE EFFICIENT WAY OF DOING THIS
         if (user.Username == "string" && user.Password == "string")
            {
                tokenA =
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmciLCJqdGkiOiI5ZjJhN2M3My03MDQ3LTQ2ZGItODNjNC1mYWIzZjNlYzA1Y2QiLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjM4LyIsImF1ZCI6IlJldk1ldHJpeCIsInJvbGUiOiJ1c2VyIiwibmJmIjoxNzMwNzYzOTU4LCJleHAiOjE3MzA3Njc1NTgsImlhdCI6MTczMDc2Mzk1OH0.XcB0zrR_7FsNmfzAPcdChiYTtvYhw2aXX2cfSQJCl9s";
                tokenB =
                    "d8E4THTgIuPbLXxo+1bXolJxhWUz3Pr0mzdme9HemBM=";
                response.StatusCode = 200;
                return new { tokenA = tokenA, tokenB = tokenB };
            }
         if (user.Username == "403Response" && user.Password == "403Response")
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
        
        if (JWT == "empty")
        {
            return new
            {
                shots = Array.Empty<SimulatedShotList>()
            };
        }

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

    static object DeleteBallHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        // Get jwt token from header
        string auth = request.Headers["Authorization"];

        string JWT = auth.Substring("Bearer ".Length).Trim();
        if (JWT == "403")
        {
            response.StatusCode = 403;
            return null;
        }
        response.StatusCode = 200;
        return null;
    }
    
    static object DeleteShotHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        // Get jwt token from header
        string auth = request.Headers["Authorization"];

        string JWT = auth.Substring("Bearer ".Length).Trim();
        if (JWT == "403")
        {
            response.StatusCode = 403;
            return null;
        }
        response.StatusCode = 200;
        return null;
    }
    
    static object GetArsenalHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        string auth = request.Headers["Authorization"];

        string JWT = auth.Substring("Bearer ".Length).Trim();

        if (JWT == "403")
        {
            response.StatusCode = 403;
            return null;
        }

        if (JWT == "empty")
        {
            return new
            {
                BallList = Array.Empty<Arsenal>()
            };
        }

        List<Ball> BallList = new List<Ball>();
        BallList.Add(new Ball("Test", 20.2, 11.3, "Pancake"));
        return new {ballList = BallList };
    }
        
    static object InsertBallHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        // Get jwt token from header
        string auth = request.Headers["Authorization"];

        string JWT = auth.Substring("Bearer ".Length).Trim();
        if (JWT == "403")
        {
            response.StatusCode = 403;
            return null;
        }
        response.StatusCode = 200;
        return null;
    }
}
