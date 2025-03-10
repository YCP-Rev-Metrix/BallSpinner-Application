using Common.POCOs;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;

namespace RevMetrix.BallSpinner.FrontEnd;

public class TestChart2 : IDataViewModel
{
    public string Name { get; set; }
    private IBallSpinner _ballSpinner;

    public ISeries[] Series { get; set; } = [
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
];
    /*
        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "My chart title",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };*/

    public Action<Metric, float, float>? DataReceived { get; set; }

    public TestChart2(IBallSpinner ballSpinner, string name, Metric metrics)
    {
        _ballSpinner = ballSpinner;
        Name = name ?? string.Empty;
        Metrics = metrics;
        Series = new ISeries[] {
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
        };
    }


    public Metric Metrics { get; }

    public void OnDataReceived(Metric metric, float value, float timeFromStart)
    {
        DataReceived?.Invoke(metric, value, timeFromStart);
    }

    public void Dispose()
    {
        _ballSpinner.DataParser.Unsubscribe(OnDataReceived);
    }

    public void Reset()
    {
        //_webView.Reload();
    }
}

