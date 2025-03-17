﻿using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace RevMetrix.BallSpinner.FrontEnd.Testing
{

    public class TestChart
    {
        public ISeries[] Series { get; set; } = [
            new ColumnSeries<int>(3, 4, 2),
            new ColumnSeries<int>(4, 2, 6),
            new ColumnSeries<double, DiamondGeometry>(4, 3, 4)
        ];
    }
}