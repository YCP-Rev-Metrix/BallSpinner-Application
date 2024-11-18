using System;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;
public class WriteToTempRevFile : IDisposable
{
    private readonly string _filePath;
    private bool _disposed = false;
    private bool _isFileCleared = false;

    public WriteToTempRevFile(string filePath)
    {
        _filePath = filePath;
    }

    public void WriteData(string[] dataArray)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WriteToTempRevFile));

        // Clear the file only once on the first write
        if (!_isFileCleared)
        {
            using (StreamWriter writer = new StreamWriter(_filePath, append: false))
            {
                writer.WriteLine(""); // Clear contents
            }
            _isFileCleared = true;
        }

        // Write data to the file
        using (StreamWriter writer = new StreamWriter(_filePath, append: true))
        using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            foreach (var record in dataArray)
            {
                csv.WriteField(record);
            }
            csv.NextRecord();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;
    }

    ~WriteToTempRevFile()
    {
        Dispose(false);
    }
}

