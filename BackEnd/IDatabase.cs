using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.BackEnd;

///<Summary>
/// Placeholder (fill in this section later)
///</Summary>
public interface IDatabase
{
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
    Task<SimulatedShotList?> GetListOfShots();
    //Task GetListOfSmartDots();
    //Task GetShotData();
    //Task RegisterSmartDot();
    //Task RegisterBallSpinner();
    Task<bool> UploadShot(string name, float InitialSpeed);
    //Task GetBalls();
    ///<Summary>
    /// Database method for getting a user registered
    ///</Summary>
    Task<Token?> RegisterUser(string firstname, string lastname, string username, string password, string email, string phonenumber);

    ///<Summary>
    /// Parses temp rev file and puts data into a SampleData list
    ///</Summary>
    public Task<List<SampleData>> GetSampleData(List<SampleData> sampleData, string path);
}