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

namespace RevMetrix.BallSpinner.FrontEnd;

class InitialValuesViewModel : INotifyPropertyChanged
{
    BallsViewModel _ballsViewModel;
    public InitialValuesChart chart = null;
    public ISeries[] Series { get; private set; }
    public ObservableCollection<Ball> Arsenal { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public Axis XAxis { get; private set; }
    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            MinLimit = 0,
            MaxLimit = 800,
            CustomSeparators = new double[] { 0, 100, 200, 300, 400, 500, 600, 700, 800 },
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
        chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3]);
        Series = chart.Series;
        
    }

    public void OnGraphChanged(Coordinates lower, Coordinates inflection, Coordinates upper)
    {
        InitialValuesModel model = new InitialValuesModel();
        List<List<double>> axes = model.CalculateBezierCurve(lower, inflection, upper);
        chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3]);
        Series = chart.Series;
        OnPropertyChanged(nameof(Series));
        
    }
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
