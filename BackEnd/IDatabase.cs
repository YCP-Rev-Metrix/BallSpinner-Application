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
    Task LoginUser();
    //Task DeleteSmartDot();
    //Task DeleteBallSpinner();
    //Task DeleteShot();
    //Task DeleteUser();
    //Task GetListOfBallSpinners();
    //Task GetListOfShots();
    //Task GetListOfSmartDots();
    //Task GetShotData();
    //Task RegisterSmartDot();
    //Task RegisterBallSpinner();
    //Task UploadShot();
    //Task GetBalls();
    ///<Summary>
    /// Database method for getting a user registered
    ///</Summary>
    Task Register();
}