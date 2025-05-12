using RevMetrix.BallSpinner.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using Common.POCOs;
using System.Collections.ObjectModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using CommunityToolkit.Mvvm.Input;


namespace RevMetrix.BallSpinner.FrontEnd;

partial class InitialValuesViewModel : INotifyPropertyChanged
{
    BallsViewModel _ballsViewModel;
    public InitialValuesChart chart = null;
    public ISeries[] Series { get; private set; }
    public ObservableCollection<Ball> Arsenal { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public double max;
    public double min;
    public double xratio;
    public double yratio;
    public Axis[] XAxes { get; private set; } =
    {
        new Axis 
        {
            LabelsPaint = new SolidColorPaint(SKColors.White),
            //Name = "Time (ms)",
            //NamePaint = new SolidColorPaint(SKColors.White),
        }
    };
    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            MinLimit = 0,
            MaxLimit = 800,
            CustomSeparators = new double[] { 0, 100, 200, 300, 400, 500, 600, 700, 800 },
            LabelsPaint = new SolidColorPaint(SKColors.White),
            //Name = "RPM",
            //NamePaint = new SolidColorPaint(SKColors.White),
        }
    };


    public InitialValuesViewModel(IDatabase database)
    {
        _ballsViewModel = new BallsViewModel(database);
        Arsenal = _ballsViewModel.Arsenal;
        InitGraph();
    }

    private void InitGraph()
    {
        InitialValuesModel model = new InitialValuesModel();
        Coordinates starterLower = new Coordinates(0, 0);
        Coordinates starterInflection = new Coordinates(70, 50);
        Coordinates starterUpper = new Coordinates(100, 800);
        List<List<double>> axes = model.CalculateBezierCurve(starterLower, starterInflection, starterUpper);
        chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3], Coordinates.ToList(starterInflection));
        Series = chart.Series;
        max = 800.0;
        min = 0.0;
    }

    public void OnGraphChanged(Coordinates lower, Coordinates inflection, Coordinates upper)
    {
        InitialValuesModel model = new InitialValuesModel();
        List<List<double>> axes = model.CalculateBezierCurve(lower, inflection, upper);
        chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3], Coordinates.ToList(inflection));
        Series = chart.Series;
        OnPropertyChanged(nameof(Series));
        min=lower.y;
        max=upper.y;
    }
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public Coordinates GetInflection()
    {
        return chart.GetInflection();
    }

    public double GetXRatio()
    {
        return xratio;
    }

    public double GetYRatio()
    {
        return yratio;
    }

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args)
    {
        var argschart = (ICartesianChartView)args.Chart;

        // scales the UI coordinates to the corresponding data in the chart.
        var scaledPoint = argschart.ScalePixelsToData(args.PointerPosition);

        // finally add the new point to the data in our chart.
        InitialValuesModel model = new InitialValuesModel();
        Coordinates lower = new Coordinates(0, min);
        Coordinates inflection = new Coordinates(scaledPoint.X, scaledPoint.Y);
        Coordinates upper = new Coordinates(100, max);
        List<List<double>> axes = model.CalculateBezierCurve(lower, inflection, upper);
        chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3], Coordinates.ToList(inflection));
        Series = chart.Series;
        OnPropertyChanged(nameof(Series));
        xratio = (scaledPoint.X - lower.x) / (upper.x - lower.x);
        yratio = (scaledPoint.Y - lower.y) / (upper.y - lower.y);


        //Console.Out.WriteLine("Chart Clicked");
    }
}
