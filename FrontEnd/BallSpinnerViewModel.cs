﻿using Common.POCOs;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Xaml;
using Microsoft.Maui.ApplicationModel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Defaults;


namespace RevMetrix.BallSpinner.FrontEnd;
public partial class BallSpinnerViewModel : INotifyPropertyChanged, IDisposable
{
    /*public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            Name = "Time",
            NamePaint = new SolidColorPaint(SKColors.White),
            
        }
    ];*/
    public ICartesianAxis[] MagYAxes { get; set; } = [
        new Axis
        {
            MaxLimit = 1,

        }
    ];
    public ICartesianAxis[] RotYAxes { get; set; } = [
        new Axis
        {
            MaxLimit = 180,
            MinLimit = -180,
            CustomSeparators = new double[] { -180, 0, 180 },
        }
    ];
    public ICartesianAxis[] AccYAxes { get; set; } = [
        new Axis
        {
            MaxLimit = 1,
            MinLimit = -1,
        }
    ];
    public ICartesianAxis[] LigYAxes { get; set; } = [
        new Axis
        {
            MaxLimit = 0.1,
            MinLimit = 0,
        }
    ];
    public ObservableCollection<ISeries> AccelerationSeries { get; set; } 

    public ObservableCollection<ISeries> RotationSeries { get; set; }

    public ObservableCollection<ISeries> MagnetometerSeries { get; set; }

    public ObservableCollection<ISeries> LightSeries { get; set; }

    //public LineSeries<ObservablePoint> accelXSeries = new LineSeries<ObservablePoint>()
    public LineSeries<double> accelXSeries = new LineSeries<double>()
    {
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }
    };
    //public LineSeries<ObservablePoint> accelYSeries = new LineSeries<ObservablePoint>()
    public LineSeries<double> accelYSeries = new LineSeries<double>()
    {
            Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 4 }
    };
    //public LineSeries<ObservablePoint> accelZSeries = new LineSeries<ObservablePoint>()
    public LineSeries<double> accelZSeries = new LineSeries<double>()
    {
            Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 4 }
    };
    public LineSeries<double> rotatXSeries = new LineSeries<double>()
    {
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }
    };
    public LineSeries<double> rotatYSeries = new LineSeries<double>()
    {
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 4 }
    };
    public LineSeries<double> rotatZSeries = new LineSeries<double>()
    {
        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 4 }
    };
    public LineSeries<double> magneXSeries = new LineSeries<double>()
    {
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }
    };
    public LineSeries<double> magneYSeries = new LineSeries<double>() 
    {
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 4 }
    };
    public LineSeries<double> magneZSeries = new LineSeries<double>()
    {
        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 4 }
    };
    public LineSeries<double> lightSeries = new LineSeries<double>()
    {
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }
    };
    const int maxDataPoints = 50; //maximum values for the graphs
    //private readonly Stopwatch _timer = new Stopwatch();
    //public List<ObservablePoint> accX = new List<ObservablePoint>();
    //public List<ObservablePoint> accY = new List<ObservablePoint>();
    //public List<ObservablePoint> accZ = new List<ObservablePoint>();



    public bool NotConnectedFadeVisible
    {
        get => _connectedFadeVisible;
        set
        {
            _connectedFadeVisible = value;
            OnPropertyChanged(nameof(NotConnectedFadeVisible));
        }
    }
    private bool _connectedFadeVisible = true;

    public string Name 
    {
        get => _ballSpinner.Name;
        set
        {
            _ballSpinner.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public IBallSpinner BallSpinner => _ballSpinner;

    private IBallSpinner _ballSpinner;

    public ObservableCollection<double> accelXValues = new ObservableCollection<double>();
    public ObservableCollection<double> accelYValues = new ObservableCollection<double>();
    public ObservableCollection<double> accelZValues = new ObservableCollection<double>();
    public ObservableCollection<double> rotatXValues = new ObservableCollection<double>();
    public ObservableCollection<double> rotatYValues = new ObservableCollection<double>();
    public ObservableCollection<double> rotatZValues = new ObservableCollection<double>();
    public ObservableCollection<double> magneXValues = new ObservableCollection<double>();
    public ObservableCollection<double> magneYValues = new ObservableCollection<double>();
    public ObservableCollection<double> magneZValues = new ObservableCollection<double>();
    public ObservableCollection<double> lightValues = new ObservableCollection<double>();


    public IDataViewModel LeftView { get; }

    public IDataViewModel TopMiddleView { get; }

    public IDataViewModel BottomMiddleView { get; }

    public IDataViewModel TopRightView { get; }

    public IDataViewModel BottomRightView { get; }

    public bool IsSimulation { get; }
    public bool NotSimulation => !IsSimulation;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool InitialValuesSet => _ballSpinner.InitialValuesSet;

    public MainPage MainPage { get; }
    public int interval = 0;
    private FrontEnd _frontEnd;
    public BallSpinnerViewModel(FrontEnd frontend, MainPage mainPage, IBallSpinner ballspinner)
    {
        //_timer.Start();
        accelXSeries.GeometrySize = 0.5;
        accelYSeries.GeometrySize = 0.5;
        accelZSeries.GeometrySize = 0.5;
        rotatXSeries.GeometrySize = 0.5;
        rotatYSeries.GeometrySize = 0.5;
        rotatZSeries.GeometrySize = 0.5;
        magneXSeries.GeometrySize = 0.5;
        magneYSeries.GeometrySize = 0.5;
        magneZSeries.GeometrySize = 0.5;
        lightSeries.GeometrySize = 0.5;


        _frontEnd = frontend;
        MainPage = mainPage;
        _ballSpinner = ballspinner;
        IsSimulation = _ballSpinner.GetType() == typeof(Simulation);
        NotConnectedFadeVisible = !IsSimulation;

        LeftView = new BallViewModel(_ballSpinner);
        /*TopMiddleView = new GraphViewModel(_ballSpinner, "Acceleration (g)", Metric.AccelerationX | Metric.AccelerationY | Metric.AccelerationZ);
        BottomMiddleView = new GraphViewModel(_ballSpinner, "Rotation (°)", Metric.RotationX | Metric.RotationY | Metric.RotationZ);
        TopRightView = new GraphViewModel(_ballSpinner, "Magnetometer (μT)", Metric.MagnetometerX | Metric.MagnetometerY | Metric.MagnetometerZ);
        BottomRightView = new GraphViewModel(_ballSpinner, "Light (lux)", Metric.Light);*/
        TopMiddleView = new ChartViewModel(_ballSpinner, "Acceleration (g)", Metric.AccelerationX | Metric.AccelerationY | Metric.AccelerationZ);
        BottomMiddleView = new ChartViewModel(_ballSpinner, "Rotation (°)", Metric.RotationX | Metric.RotationY | Metric.RotationZ);
        TopRightView = new ChartViewModel(_ballSpinner, "Magnetometer (μT)", Metric.MagnetometerX | Metric.MagnetometerY | Metric.MagnetometerZ);
        BottomRightView = new ChartViewModel(_ballSpinner, "Light (lux)", Metric.Light);
        //SetAllSeries();
        _ballSpinner.PropertyChanged += BallSpinner_PropertyChanged;
        _ballSpinner.OnConnectionChanged += BallSpinner_OnConnectionChanged;


        //BallSpinner_OnConnectionChanged(_ballSpinner.IsConnected()); Caused double smartdot connection screen
    }

    public void SetAllSeries()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var accelSeriesList = new List<ISeries>();
            var rotatSeriesList = new List<ISeries>();
            var magneSeriesList = new List<ISeries>();

            //_timer.Stop();
            //double time = _timer.ElapsedMilliseconds*(1/1000);
            //_timer.Start();

            //for (int i = 0; i < accelXValues.Count; i++)
            //{
            /*if (accelXValues.Count > 0)
            {
                accX.Add(new ObservablePoint(time, accelXValues[accelXValues.Count - 1]));
                accY.Add(new ObservablePoint(time, accelYValues[accelYValues.Count - 1]));
                accZ.Add(new ObservablePoint(time, accelZValues[accelZValues.Count - 1]));

            }*/
            //}
            //accelXSeries.Values = accX;
            /*for (int i = 0; i < accelYValues.Count; i++)
            {
                accY.Add(new ObservablePoint(time+i, accelYValues[i]));
            }*/
            //accelYSeries.Values = accY;
            /*for (int i = 0; i < accelZValues.Count; i++)
            {
                accZ.Add(new ObservablePoint(time+i, accelZValues[i]));
            }*/
            //accelZSeries.Values = accZ;
            accelXSeries.Values = accelXValues;
            accelYSeries.Values = accelYValues;
            accelZSeries.Values = accelZValues;
            rotatXSeries.Values = rotatXValues;
            rotatYSeries.Values = rotatYValues;
            rotatZSeries.Values = rotatZValues;
            magneXSeries.Values = magneXValues;
            magneYSeries.Values = magneYValues;
            magneZSeries.Values = magneZValues;
            lightSeries.Values = lightValues;
            //Console.Out.WriteLine(rotatXValues[0]);  uncomment for brandon testing of rotat values.

            AccelerationSeries = //new ObservableCollection<ISeries> 
            [
                accelXSeries, accelYSeries, accelZSeries
            ];
            RotationSeries = //new ObservableCollection<ISeries>
            [
                rotatXSeries, rotatYSeries, rotatZSeries
            ];
            MagnetometerSeries = //new ObservableCollection<ISeries>
            [
                magneXSeries, magneYSeries, magneZSeries
            ];
            LightSeries = [lightSeries];
            if (accelXValues.Count > maxDataPoints)
            {
                accelXValues.RemoveAt(0);
                accelYValues.RemoveAt(0);
                accelZValues.RemoveAt(0);
            }
            if (rotatXValues.Count > maxDataPoints)
            {
                rotatXValues.RemoveAt(0);
                rotatYValues.RemoveAt(0);
                rotatZValues.RemoveAt(0);
            }

            if (magneXValues.Count > maxDataPoints)
            {
                magneXValues.RemoveAt(0);
                magneYValues.RemoveAt(0);
                magneZValues.RemoveAt(0);
            }

            if (lightValues.Count > maxDataPoints)
            {
                lightValues.RemoveAt(0);
            }


            /*Console.Out.WriteLine("TESTING");
            for(int i = 0; i < magneXSeries.Values.Count; i++)
            {
                Console.Out.WriteLine(magneXValues[i].ToString());
            }*/
            //if (interval > 5)
            //{
            OnPropertyChanged(nameof(MagnetometerSeries));
            OnPropertyChanged(nameof(AccelerationSeries));
            OnPropertyChanged(nameof(RotationSeries));
            OnPropertyChanged(nameof(LightSeries));
            //    interval = 0;
            //}
            //else interval++;

            // };
        });




        //for(int i = 0; i < ballspinner.timeFromStart; i++)
        //{
        //    accelX.Add((double)Metric.AccelerationX);
        //}


    }

    private async void BallSpinner_OnConnectionChanged(bool connected)
    {
        NotConnectedFadeVisible = !connected;

        if(connected && string.IsNullOrEmpty(_ballSpinner.SmartDotMAC))
        {
            await _frontEnd.ConnectSmartDot(_ballSpinner);
        }
    }

    public async void ConnectSmartDot()
    {
        await _frontEnd.ConnectSmartDot(_ballSpinner);
    }

    private void UpdateConnected(bool connected)
    {
        NotConnectedFadeVisible = !connected | string.IsNullOrEmpty(_ballSpinner.SmartDotMAC);
    }

    private void BallSpinner_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    public void Start()
    {
        _ballSpinner.Start();
    }

    public void Stop()
    {
        _ballSpinner.Stop();
    }
    public void Reconnect()
    {
        _ballSpinner.InitializeConnection();
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _ballSpinner.PropertyChanged -= BallSpinner_PropertyChanged;
        _ballSpinner.OnConnectionChanged -= BallSpinner_OnConnectionChanged;

        DisconnectFromBSC();

        _ballSpinner.Dispose();
    }
    private void DisconnectFromBSC()
    {
        //If the ballspinner is TCP connection based as a BallSpinnerClass then we need to disconnect it when we turn off 
        BallSpinnerClass bs = BallSpinner as BallSpinnerClass;
        if (BallSpinner is BallSpinnerClass spinner)
        {
            TCP conn = spinner.GetConnection();
            if (conn != null)
            {
                conn.DisconnectFromBSC();
                Debug.WriteLine("Sent Disconnect REquest from bsa to bsc");
            }

        }
    }
    public void OpenSmartDotSettings()
    {
        _frontEnd.SmartDotSettings(this);
    }

    public void PointerDown(PointerCommandArgs args)
    {
        var chart = (ICartesianChartView)args.Chart;

        // scales the UI coordinates to the corresponding data in the chart.
        var scaledPoint = chart.ScalePixelsToData(args.PointerPosition);

        Console.Out.WriteLine("Pointer Detected");
    }
    //TODO: Function call to Backend to get ODR & sample rates

    //TODO: Function call to Backend to pass selected sample rates
}
