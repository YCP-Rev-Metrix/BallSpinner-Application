namespace RevMetrix.BallSpinner.BackEnd.Common.POCOs;

public class Credentials
{
    /// <summary>
    /// This poco is used for login purposes
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public Credentials(string username, string password)
    {
        Username = username;
        Password = password;
    }
    
    /// <summary>
    /// Username set by user
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Password set by user
    /// </summary>
    public string? Password { get; set; }
}