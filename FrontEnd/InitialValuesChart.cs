using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using RevMetrix.BallSpinner.BackEnd;
using System.Collections.Generic;

namespace RevMetrix.BallSpinner.FrontEnd;

public class InitialValuesChart
{
    public ISeries[] Series { get; set; } 
    
    public InitialValuesChart() { 
        Series = [
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

    }

    public InitialValuesChart(List<double> x, List<double> y, List<double> bezierX, List<double> bezierY)
    {
        var seriesList = new List<ISeries>();

        List<ObservablePoint> lineValues = new List<ObservablePoint>();
        List<ObservablePoint> bezierValues = new List<ObservablePoint>();

        for (int i = 0; i < x.Count; i++)
        {
            lineValues.Add(new ObservablePoint(x[i], y[i]));
            bezierValues.Add(new ObservablePoint(bezierX[i], bezierY[i]));
        }

        seriesList.Add(new LineSeries<ObservablePoint>()
        {
            Values = lineValues
        });
        seriesList.Add(new LineSeries<ObservablePoint>()
        {
            Values = bezierValues
        });


        Series = seriesList.ToArray();
    }

}
    /*
        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "My chart title",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };*/


