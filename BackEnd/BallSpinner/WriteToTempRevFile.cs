using System;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;
public class WriteToTempRevFile : IDisposable
{
    private readonly string _filePath;

    private StreamWriter? _writer;
    private CsvWriter? _csvWriter;

    public WriteToTempRevFile(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Indicates that writing has started
    /// </summary>
    public void Start()
    {
        string? directory = Path.GetDirectoryName(_filePath);

        if (directory != null)
            Directory.CreateDirectory(directory);

        _writer = File.CreateText(_filePath);
        _csvWriter = new CsvWriter(_writer, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Indicates that writing has stopped
    /// </summary>
    public void Stop()
    {
        Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _csvWriter?.Dispose();

        _writer = null;
        _csvWriter = null;
    }

    /// <summary>
    /// Write data into the rev file
    /// </summary>
    public void WriteData(string[] dataArray)
    {
        if (_writer == null || _csvWriter == null)
            throw new Exception("Trying to write to temp file without opening it first");

        foreach (var record in dataArray)
        {
            _csvWriter.WriteField(record);
        }

        _csvWriter.NextRecord();
        _csvWriter.Flush();
    }
}

