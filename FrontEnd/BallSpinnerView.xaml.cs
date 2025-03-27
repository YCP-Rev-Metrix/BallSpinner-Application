using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System.Diagnostics;
using LiveChartsCore.SkiaSharpView.Maui;
using LiveChartsCore.Kernel.Sketches;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;



namespace RevMetrix.BallSpinner.FrontEnd;

public partial class BallSpinnerView : ContentView
{
    public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            Name = "Time",
        }
    ];



    
    private BallSpinnerViewModel _viewModel = null!;

	public BallSpinnerView()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        _viewModel = (BallSpinnerViewModel)BindingContext;
        _viewModel.LeftView.DataReceived += (metric, value, timeFromStart) => { DataReceived(_viewModel.LeftView, LeftView, metric, value, timeFromStart); };
        _viewModel.TopMiddleView.DataReceived += (metric, value, timeFromStart) => { ChartDataReceived(_viewModel.TopMiddleView, TopMiddleView, metric, value, timeFromStart); };
        _viewModel.TopRightView.DataReceived += (metric, value, timeFromStart) => { ChartDataReceived(_viewModel.TopRightView, TopRightView, metric, value, timeFromStart); };
        _viewModel.BottomMiddleView.DataReceived += (metric, value, timeFromStart) => { ChartDataReceived(_viewModel.BottomMiddleView, BottomMiddleView, metric, value, timeFromStart); };
        _viewModel.BottomRightView.DataReceived += (metric, value, timeFromStart) => { ChartDataReceived(_viewModel.BottomRightView, BottomRightView, metric, value, timeFromStart); };
    
    }

    private void OnRemoveBallSpinnerButton(object sender, EventArgs args)
    {       
        _viewModel.MainPage.RemoveBallSpinner(_viewModel);
    }

    private void SelectSmartDotButton(object sender, EventArgs args)
    {
        _viewModel.ConnectSmartDot();
    }

    private void DataReceived(IDataViewModel model, WebView webview, Metric metric, float value, float timeFromStart)
    {
        if (!model.Metrics.HasFlag(metric))
            return;

        try
        {

            //Need to switch to a web connection, this is crazy slow!
            webview.Eval($"window.data('{metric}',{value}, {timeFromStart})");
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void ChartDataReceived(IDataViewModel model, CartesianChart chart, Metric metric, float value, float timeFromStart)
    {
        if (!model.Metrics.HasFlag(metric))
            return;

        try
        {
            UpdateChart(metric, value);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void Reconnect_Clicked(object sender, EventArgs e)
    {
        _viewModel.Reconnect();
    }

    private void OnSettingsButtonClicked(object sender, EventArgs e)
    {
        _viewModel.OpenSmartDotSettings();
    }

    private void UpdateChart(Metric metric, float value)
    {


            int length = metric.ToString().Length;
            char first = metric.ToString()[0];

            char final = metric.ToString()[length - 1];
            //Console.Out.WriteLine(metric.ToString());

            if (final == 'Y')
            {
                //Console.Out.WriteLine("Testing Y");
                switch (first)
                {
                    case 'A':
                        _viewModel.accelYValues.Add((double)value);
                        //for (int i = 0; i < _viewModel.accelYValues.Count; i++)
                        //{
                        //    Console.Out.WriteLine(_viewModel.accelYValues[i].ToString());
                        //}
                        break;
                    case 'R':
                        _viewModel.rotatYValues.Add((double)value);
                        break;
                    case 'M':
                        _viewModel.magneYValues.Add((double)value);
                        break;
                }
            }
            else if (final == 'Z')
            {
                switch (first)
                {
                    case 'A':
                        _viewModel.accelZValues.Add((double)value);
                        break;
                    case 'R':
                        _viewModel.rotatZValues.Add((double)value);
                        break;
                    case 'M':
                        _viewModel.magneZValues.Add((double)value);
                        break;

                }
            }
            else
            {
                switch (first)
                {
                    case 'A':
                        _viewModel.accelXValues.Add((double)value);
                        break;
                    case 'R':
                        _viewModel.rotatXValues.Add((double)value);
                        break;
                    case 'M':
                        _viewModel.magneXValues.Add((double)value);
                        break;
                    case 'L':
                        _viewModel.lightValues.Add((double)value);
                        break;
                }
            }
            _viewModel.SetAllSeries();
       
    }
}