using Common.POCOs;

namespace RevMetrix.BallSpinner.Tests;

public class TestWriteToTempFile() : TestBase
{

    [Fact]
    private void SimpleTest() 
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
        TempFileWriter.Stop();
        // Use GetSampleData to view content of file
        List<SampleData> list = new List<SampleData>();
        Database.GetSampleData(list, revFilePath);
        // Test to make sure dataArray1 is parsed correctly into sample and is 1st element
        Assert.Equal("3", list[0].Type);
        Assert.Equal(43, list[0].Count);
        Assert.Equal(5.0, list[0].Logtime);
        Assert.Equal(434.212, list[0].X);
        Assert.Equal(4342.2, list[0].Y);
        Assert.Equal(23423, list[0].Z);
        // Test to make sure dataArray2 is parsed correctly into sample and is 2nd element
        Assert.Equal("2", list[1].Type);
        Assert.Equal(4, list[1].Count);
        Assert.Equal(52.0f, list[1].Logtime);
        Assert.Equal(112.4124, list[1].X);
        Assert.Equal(4342412.2, list[1].Y);
        Assert.Equal(44, list[1].Z);
    }
}