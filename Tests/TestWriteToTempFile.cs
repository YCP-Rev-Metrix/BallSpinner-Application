using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

namespace RevMetrix.BallSpinner.Tests;

public class TestWriteToTempFile() : TestBase
{

    [Fact]
    private async void SimpleTest() 
    {
        TempFileWriter.Start();
        string[] dataArray = new string[6] 
        {
            "3", "5.0", "43", "434.212", "4342.2", "23423"
        };
        
        TempFileWriter.WriteData(dataArray);
        
        string[] dataArray2 =
        {
            "2", "52.0", "4", "112.4124", "4342412.2", "44"
        };
                
        TempFileWriter.WriteData(dataArray2);
        // Use GetSampleData to view content of file
        List<SampleData> list = new List<SampleData>();
        await Database.GetSampleData(list, revFilePath, 2, DataParser.NUM_DATA_POINTS);
        // Dispose of memory mapped file
        TempFileWriter.Stop();
        // Test to make sure dataArray1 is parsed correctly into sample and is 1st element
        Assert.Equal("3", list[0].Type);
        Assert.Equal(43, list[0].Count);
        Assert.Equal(5.0, list[0].Logtime);
        Assert.InRange((double) list[0].X, 434.212 - epsilon, 434.212 + epsilon);
        Assert.InRange((double)list[0].Y, 4342.2 - epsilon, 4342.2 + epsilon);
        Assert.Equal(23423, list[0].Z);
        // Test to make sure dataArray2 is parsed correctly into sample and is 2nd element
        Assert.Equal("2", list[1].Type);
        Assert.Equal(4, list[1].Count);
        Assert.InRange((double)list[1].X, 112.4124 - epsilon, 112.4124 + epsilon);
        Assert.InRange((double)list[1].Y, 4342412.2 - epsilon, 4342412.2 + epsilon);
        Assert.Equal(52.0f, list[1].Logtime);
        Assert.Equal(44, list[1].Z);
    }
}