using System.Security.AccessControl;
using RevMetrix.BallSpinner.BackEnd.Common.Utilities;
using Common.POCOs;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
namespace RevMetrix.BallSpinner.BackEnd.Common.Utilities;
/// <summary>
/// Contains many common utility functions
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Returns the path of the temporary rev file used for caching simulation data for a given BallSpinner. Does not
    /// create directory, that is up to the calling function.
    /// </summary>
    public static string GetTempRevFileDir(string BallSpinnerName)
    {
        string tempFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "RevMetrix", $"{BallSpinnerName} Temp.csv");
        return tempFilePath;
    }
    /// <summary>
    /// Returns the path of where the current users local rev file should be saved.
    /// </summary>
    public static string GetLocalRevFileDir(string CurrentUser, string FileName)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "RevMetrix", "LocalRevFiles", CurrentUser, $"{FileName}.csv");
        return path;
    }
    /// <summary>
    /// Method used for saving local rev file to user's directory system. Throws exception if file already exists, or
    /// temp rev file has not yet been initialized.
    /// </summary>
    public static void SaveLocalRevFile (string FileName, string BallSpinnerName, string CurrentUser)
    {
        // Get the directory that corresponds to the current user for the given file name.
        string localRevFilePath = GetLocalRevFileDir(CurrentUser, FileName);
            
        // Check to see if file already exists
        if (File.Exists(localRevFilePath))
        {
            throw new Exception("File already exists");
        }
            
        // Get the contents of the csv file local csv file, place into poco
        List<SampleData> data = new List<SampleData>();
        // Declare path to temp file (should already be created)
        string tempFilePath = GetTempRevFileDir(BallSpinnerName);
            
        // Ensure temp rev file already exists
        if (!File.Exists(tempFilePath))
        {
            throw new Exception("Error. Attempting to save a shot that has not been initialized.");
        }
            
        // Get data, store in poco and sort by time ascending
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord  = false,
        };
        using (var reader = new StreamReader(tempFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            while (csv.Read())
            {
                var record = csv.GetRecord<SampleData>();
                data.Add(record);
            }
        }
        // sort the data by time (ascending)
        // NEED A BETTER WAY TO DO THIS! PREFERABLY WITH CSVHELPER
        data.Sort((x, y) => Nullable.Compare(x.Logtime, y.Logtime));
            
        // Create directory for new file (if not created)
        string? directory = Path.GetDirectoryName(localRevFilePath);
        if (directory != null)
            Directory.CreateDirectory(directory);
            
        // Write the contents of the temp rev file into the local rev file. Wrap in 'using' so resources are 
        // deallocated when done.
        using (var writer = File.CreateText(localRevFilePath))
        using (var csvWriter = new CsvWriter(writer, config))
        {
            // Write records to the csv
            csvWriter.WriteRecords(data);
        }
    }
}