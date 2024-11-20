namespace RevMetrix.BallSpinner.BackEnd.Common.POCOs;
///<Summary>
/// Placeholder (fill in this section later)
///</Summary>
public class SampleData
{
    public SampleData() { }

    public SampleData(string? type, float? timestamp, int? count, float? x, float? y, float? z)
    {
        this.type = type;
        this.count = count;
        logtime = timestamp;
        X = x;
        Y = y;
        Z = z;
    }
        
    public string? type { get; set; }

    public float? logtime { get; set; }

    public int? count { get; set; }

    public float? X { get; set; }
    public float? Y { get; set; }
    public float? Z { get; set; }

}