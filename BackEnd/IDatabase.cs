using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.BackEnd;

///<Summary>
/// Placeholder (fill in this section later)
///</Summary>
public interface IDatabase
{
    /// <summary>
    /// Event invoked when the user logs in or logs out
    /// </summary>
    event Action<bool>? OnLoginChanged;

    // Define database methods
    ///<Summary>
    /// Database method for logging in a user
    ///</Summary>
    Task<Token?> LoginUser(string username, string password);
    //Task DeleteSmartDot();
    //Task DeleteBallSpinner();
    //Task DeleteShot();
    //Task DeleteUser();
    //Task GetListOfBallSpinners();
    /// <summary>
    /// Returns a list of shots for the current user
    /// </summary>
    Task<SimulatedShotList?> GetListOfShots();
    //Task GetListOfSmartDots();
    //Task GetShotData();
    //Task RegisterSmartDot();
    //Task RegisterBallSpinner();
    /// <summary>
    /// Uploads a shot for the current user. The shot will be parsed from whatever is in the TempRev.csv file.
    /// </summary>
    Task<bool> UploadShot(IBallSpinner ballSpinner, string name, float InitialSpeed);

    //Task GetBalls();
    ///<Summary>
    /// Database method for getting a user registered
    ///</Summary>
    Task<Token?> RegisterUser(string firstname, string lastname, string username, string password, string email, string phonenumber);

    ///<Summary>
    /// Parses temp rev file and puts data into a SampleData list
    ///</Summary>
    public Task<List<SampleData>> GetSampleData(List<SampleData> sampleData, string path);
    ///<Summary>
    /// Sets user tokens for a session
    ///</Summary>
    public void SetUserTokens(Token token);
}