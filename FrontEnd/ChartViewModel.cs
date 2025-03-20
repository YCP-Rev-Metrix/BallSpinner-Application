using Common.POCOs;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

namespace RevMetrix.BallSpinner.FrontEnd;

public class ChartViewModel : GraphDataViewModel
{
    public string Name { get; set; }
    private IBallSpinner _ballSpinner;



    //private List<double> _xpoints;
    //private List<double> _ypoints;
    //private List<double> _zpoints;
    //private LineSeries<double> _x;
    //private LineSeries<double> _y;
    //private LineSeries<double> _z;


    //public ObservableCollection<ISeries> Series { get; set; } 

    /*
        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "My chart title",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };*/

    public Action<Metric, float, float>? DataReceived { get; set; }

    public ChartViewModel(IBallSpinner ballSpinner, string name, Metric metrics) : base(ballSpinner)
    {
        Name = name ?? string.Empty;
        Metrics = metrics;
        /*Series = new ISeries[] {
            new LineSeries<double>
            {
                Values = [2, 1, 3, 5, 3, 4, 6],
                Fill = null,
                GeometrySize = 20
            },
            new LineSeries<int, StarGeometry>
            {
                Values = [4, 2, 5, 2, 4, 5, 3],
                Fill = null,
                GeometrySize = 20,
            }
        };*/
    }


    public override Metric Metrics { get; }

   /* public void OnDataReceived(Metric metric, float value, float timeFromStart)
    {
        Console.Out.WriteLine("TEST");

        DataReceived?.Invoke(metric, value, timeFromStart);
        Console.Out.WriteLine("TEST");

        int length = metric.GetType().ToString().Length;
        char final = metric.GetType().ToString()[length-1];
        if (final == 'Y')
        {
            _ypoints.Add((double)metric);
            _y.Values = _ypoints;
        }
        else if (final == 'Z')
        {
            _zpoints.Add((double)metric);
            _z.Values = _zpoints;
        }
        else
        {
            _xpoints.Add((double)metric);
            _x.Values = _xpoints;
        }
        Series = [_x,_y,_z];

    }*/

    public void Dispose()
    {
        _ballSpinner.DataParser.Unsubscribe(OnDataReceived);
    }

    public void Reset()
    {
        //_webView.Reload();
    }
}

