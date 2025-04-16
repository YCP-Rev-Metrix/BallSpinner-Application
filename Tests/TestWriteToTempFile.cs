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

    [Fact]
    private async void ComprehensiveTesting()
    {
        TempFileWriter.Start();

        string[] dataArray1 = { "3", "5.0", "43", "434.212", "4342.2", "23423" };
        string[] dataArray2 = { "2", "52.0", "4", "112.4124", "4342412.2", "44" };
        string[] dataArray3 = { "1", "0.0", "0", "0", "0", "0" };
        string[] dataArray4 = { "4", "99.9", "999", "123.456", "789.1011", "1122" };
        string[] dataArray5 = { "5", "15.5", "8", "-42.42", "-313.13", "-1000" };

        TempFileWriter.WriteData(dataArray1);
        TempFileWriter.WriteData(dataArray2);
        TempFileWriter.WriteData(dataArray3);
        TempFileWriter.WriteData(dataArray4);
        TempFileWriter.WriteData(dataArray5);

        List<SampleData> list = new List<SampleData>();
        await Database.GetSampleData(list, revFilePath, 5, DataParser.NUM_DATA_POINTS);
        TempFileWriter.Stop();

        Assert.Equal(5, list.Count);

        // dataArray1
        Assert.Equal("3", list[0].Type);
        Assert.Equal(43, list[0].Count);
        Assert.Equal(5.0, list[0].Logtime);
        Assert.InRange((double)list[0].X, 434.212 - epsilon, 434.212 + epsilon);
        Assert.InRange((double)list[0].Y, 4342.2 - epsilon, 4342.2 + epsilon);
        Assert.Equal(23423, list[0].Z);

        // dataArray2
        Assert.Equal("2", list[1].Type);
        Assert.Equal(4, list[1].Count);
        Assert.Equal(52.0, list[1].Logtime);
        Assert.InRange((double)list[1].X, 112.4124 - epsilon, 112.4124 + epsilon);
        Assert.InRange((double)list[1].Y, 4342412.2 - epsilon, 4342412.2 + epsilon);
        Assert.Equal(44, list[1].Z);

        // dataArray3 - zero values test
        Assert.Equal("1", list[2].Type);
        Assert.Equal(0, list[2].Count);
        Assert.Equal(0.0, list[2].Logtime);
        Assert.Equal(0.0, list[2].X);
        Assert.Equal(0.0, list[2].Y);
        Assert.Equal(0, list[2].Z);

        // dataArray4 - large count and float values
        Assert.Equal("4", list[3].Type);
        Assert.Equal(999, list[3].Count);
        Assert.Equal(99.9, list[3].Logtime);
        Assert.InRange((double)list[3].X, 123.456 - epsilon, 123.456 + epsilon);
        Assert.InRange((double)list[3].Y, 789.1011 - epsilon, 789.1011 + epsilon);
        Assert.Equal(1122, list[3].Z);

        // dataArray5 - negative numbers test
        Assert.Equal("5", list[4].Type);
        Assert.Equal(8, list[4].Count);
        Assert.Equal(15.5, list[4].Logtime);
        Assert.InRange((double)list[4].X, -42.42 - epsilon, -42.42 + epsilon);
        Assert.InRange((double)list[4].Y, -313.13 - epsilon, -313.13 + epsilon);
        Assert.Equal(-1000, list[4].Z);
    }
}