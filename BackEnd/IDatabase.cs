using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.Database;

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
    
    ///<Summary>
    /// Database method for getting a user registered
    ///</Summary>
    Task<Token?> RegisterUser(string firstname, string lastname, string username, string password, string email, string phonenumber);

    ///<Summary>
    /// Parses temp rev file and puts data into a SampleData list
    ///</Summary>
    public Task<List<SampleData>> GetSampleData(List<SampleData> sampleData, string path, int NumRecords);
    ///<Summary>
    /// Sets user tokens for a session
    ///</Summary>
    public void SetUserTokens(Token token);

    ///<Summary>
    /// Uploads the metadata for a locally saved shot to the database for future indexing.
    /// Throws exception on Http error. In the future, this will take/provide more detailed metadata.
    /// Throws HttpException if response indicates failure.
    ///</Summary>
    Task<bool> SaveLocalEntry(string ShotName);

    ///<Summary>
    /// Uploades a new bowling ball to the database for the user that is currently logged in.
    /// Throws HttpException if response indicates failure.
    ///</Summary>
    Task<bool> AddBowlingBall(Ball ball);

    ///<Summary>
    /// Retrives a list containing the user's arsenal. Throws HttpException if response indicates failure.
    ///</Summary>
    Task<Arsenal?> GetArsenal();

    ///<Summary>
    /// Retrieves the input values that were given to a shot, for the user selected shot.
    /// Throws HttpException if response indicates failure.
    ///</Summary>
    Task<ShotInfo?> GetInitialValuesForShot(string shotName);

    ///<Summary>
    /// Retrieves a list containing the user's local shots. Throws HttpException if response indicates failure.
    ///</Summary>
    Task<LocalShotList?> GetListOfSavedLocalShots();

    ///<Summary>
    /// Deletes the bowling ball provided by the user. Throws HttpException if response indicates failure.
    ///</Summary>
    Task<bool> DeleteBowlingBall(string BallName);

    ///<Summary>
    /// Deletes the SimulatedShot provided by the user. Throws HttpException if response indicates failure.
    ///</Summary>
    Task<bool> DeleteUserShot(string? ShotName);
}