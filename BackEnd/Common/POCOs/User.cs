namespace RevMetrix.BallSpinner.BackEnd.Common.POCOs;
///<Summary>
/// Basic Poco for User
///</Summary>
public class User
{
    /// <summary>
    /// User constructor
    /// </summary>
    /// <param name="firstname"></param>
    /// <param name="lastname"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    public User(string firstname, string lastname, string username, string password, string email, string phoneNumber)
    {
        this.firstname = firstname;
        this.lastname = lastname;
        this.username = username;
        this.password = password;
        this.email = email;
        this.phoneNumber = phoneNumber;
    }
    /// <summary>
    /// Get and set firstname
    /// </summary>
    public string? firstname { get; set; } 
    /// <summary>
    /// Get and set lastname
    /// </summary>
    public string? lastname { get; set; } 
    /// Get and set Username
    /// </summary>
    public string? username { get; set; } 
    /// <summary>
    /// Get and set password
    /// </summary>
    public string? password { get; set; } 
    /// <summary>
    /// Get and set Email
    /// </summary>
    public string? email { get; set; } 
    /// <summary>
    /// Get and set Phone
    /// </summary>
    public string? phoneNumber { get; set; } 
}