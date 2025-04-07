using System.IO.MemoryMappedFiles;
using System.Text;

namespace RevMetrix.BallSpinner.BackEnd.BallSpinner;
public class WriteToTempRevFile : IDisposable
{
    private readonly string _fileName;

    private MemoryMappedFile _file;

    private int position = 0; // Track write position

    public event Action OnRecordAdded;

    public WriteToTempRevFile(string fileName)
    {
        _fileName = fileName;
    }

    /// <summary>
    /// Indicates that writing has started
    /// </summary>
    public void Start()
    {
        // Open a new memory mapped file. Set capacity to 1 mb
        _file = MemoryMappedFile.CreateOrOpen(_fileName, 1024 * 1024);
    }

    /// <summary>
    /// Indicates that writing has stopped and file data is no longer needed.
    /// </summary>
    public void Stop()
    {
        Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _file.Dispose();
    }

    /// <summary>
    /// Write data into the rev file
    /// </summary>
    public void WriteData(string[] dataArray)
    {
        // Create a view accessor for reading/writing
        using (var accessor = _file.CreateViewAccessor())
        {
            for (int i = 0; i < dataArray.Length; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(dataArray[i]);
                int length = bytes.Length;

                // Write length first (so we can read properly)
                accessor.Write(position, length);
                position += sizeof(int); // Move forward

                // Write actual string bytes
                accessor.WriteArray(position, bytes, 0, bytes.Length);
                position += length; // Move forward
            }
            OnRecordAdded.Invoke();
            Console.WriteLine("Data written to memory-mapped file.");
        }
    }
}

