using RevMetrix.BallSpinner.BackEnd.Common.POCOs;

namespace RevMetrix.BallSpinner.Tests;

public class TestWriteToTempFile() : TestBase
{

    [Fact]
    private void SimpleTest() 
    {
        TempFileWriter.Start();
        string[] dataArray = new string[6] 
        {
            "Accelerometer", "5.0", "43", "434.212", "4342.2", "23423"
        };
        
        TempFileWriter.WriteData(dataArray);
        
        string[] dataArray2 =
        {
            "Gyroscope", "52.0", "4", "112.4124", "4342412.2", "44"
        };
                
        TempFileWriter.WriteData(dataArray2);
        TempFileWriter.Stop();
        // Use GetSampleData to view content of file
        List<SampleData> list = new List<SampleData>();
        Database.GetSampleData(list, revFilePath);
        // Test to make sure dataArray1 is parsed correctly into sample and is 1st element
        Assert.Equal("Accelerometer", list[0].type);
        Assert.Equal(43, list[0].count);
        Assert.Equal(5.0f, list[0].logtime);
        Assert.Equal((float) 434.212, list[0].X);
        Assert.Equal((float) 4342.2, list[0].Y);
        Assert.Equal((float)23423, list[0].Z);
        // Test to make sure dataArray2 is parsed correctly into sample and is 2nd element
        Assert.Equal("Gyroscope", list[1].type);
        Assert.Equal(4, list[1].count);
        Assert.Equal(52.0f, list[1].logtime);
        Assert.Equal((float) 112.4124, list[1].X);
        Assert.Equal((float) 4342412.2, list[1].Y);
        Assert.Equal((float)44, list[1].Z);
    }
}