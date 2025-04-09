using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using RevMetrix.BallSpinner.BackEnd;
using SkiaSharp;
using System.Collections.Generic;

namespace RevMetrix.BallSpinner.FrontEnd;

public class InitialValuesChart
{
    public ISeries[] Series { get; set; }
    public List<ObservablePoint> lineValues;
    public List<ObservablePoint> bezierValues;
    public List<ObservablePoint> inflectionPoint;

    public InitialValuesChart(List<double> x, List<double> y, List<double> bezierX, List<double> bezierY, List<double> inflection)
    {
        var seriesList = new List<ISeries>();
        
        //List<ObservablePoint> placeholder = new List<ObservablePoint>();
        //List<ObservablePoint> placeholder2 = new List<ObservablePoint>();

        lineValues = new List<ObservablePoint>();
        bezierValues = new List<ObservablePoint>();
        inflectionPoint = new List<ObservablePoint>();

        for (int i = 0; i < x.Count; i++)
        {
            lineValues.Add(new ObservablePoint(x[i], y[i]));
            bezierValues.Add(new ObservablePoint(bezierX[i], bezierY[i]));
        }
        inflectionPoint.Add(new ObservablePoint(inflection[0],inflection[1]));
        /*placeholder.Add(new ObservablePoint(100, 800));
        placeholder2.Add(new ObservablePoint(0, 0));*/

        seriesList.Add(new LineSeries<ObservablePoint>()
        {
            Values = lineValues,
            GeometrySize = 0,
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 },

        });
        seriesList.Add(new LineSeries<ObservablePoint>()
        {
            Values = bezierValues,
            Fill = null,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 4 },
        });
        seriesList.Add(new LineSeries<ObservablePoint>()
        {
            Values = inflectionPoint,
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 4 },
            GeometrySize = 5,
            GeometryStroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 4 },
        });
        /*seriesList.Add(new LineSeries<ObservablePoint>()
        {
            Values = placeholder,
            GeometrySize = 0,
        });
        seriesList.Add(new LineSeries<ObservablePoint>()
        {
            Values = placeholder2,
            GeometrySize = 0,
        });*/




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


