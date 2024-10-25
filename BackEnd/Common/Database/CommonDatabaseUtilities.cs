using RevMetrix.BallSpinner.BackEnd;
namespace RevMetrix.BallSpinner.BackEnd.Database
{
    public partial class Database: IDatabase
    {
        private static readonly HttpClient Client = new HttpClient();
    }
}