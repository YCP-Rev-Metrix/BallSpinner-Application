using System.Globalization;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

using Xunit;
using CsvHelper;
using CsvHelper.Configuration;

namespace RevMetrix.BallSpinner.Tests;

public class TestWriteToTempFile() : TestBase
{

    [Fact]
    private void SimpleTest() 
    {
        string[] dataArray = new string[6] 
        {
            "1", "5", "43", "434.212", "4342.2", "23423"
        };
        
        TempFileWriter.WriteData(dataArray);
        
        string[] dataArray2 =
        {
            "3", "52", "4", "112.4124", "4342412.2", "44"
        };
                
        TempFileWriter.WriteData(dataArray2);
        // For now, I test this by viewing the contents of the csv
        /*
        // Test the contents of the test csv
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord  = false,
        };

        using (var reader = new StreamReader(projectPath + "/TestRevFile.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<>();
            int recordCount = records.Count();
            // Make sure there are the correct number of records in the csv
            Assert.Equal(dataArray.Length, recordCount);
            Assert.Equal(dataArray2.Length, recordCount);
        }
        */
    }
}