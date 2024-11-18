namespace RevMetrix.BallSpinner.BackEnd.Common.Utilities;
/// <summary>
/// Contains many common utility functions
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Returns the path of the %AppData% environment used for environment folders and directories
    /// </summary>
    public static string GetTempDir()
    {
        // Step 1: Get the %appdata% path
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // Step 2: Create a subdirectory for your application (optional)
        string appDirectory = Path.Combine(appDataPath, "RevMetrix");
        Directory.CreateDirectory(appDirectory);
        
        return appDirectory;
    }
    
}