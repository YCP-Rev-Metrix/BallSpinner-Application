using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;

internal class SmartDotSettingsViewModel
{
    private IBallSpinner _ballSpinner = null;
    public ObservableCollection<List<double>> Ranges { get; private set; }
    public ObservableCollection<List<double>> SampleRates { get; private set; }

    public SmartDotSettingsViewModel(IBallSpinner ballSpinner)
    {
        _ballSpinner = ballSpinner;

        Ranges = new ObservableCollection<List<double>>();
        SampleRates = new ObservableCollection<List<double>>();

        BuildRanges();
        BuildSampleRates();
    }

    public void BuildRanges()
    {
        Ranges.Clear();

        var RangesList = _ballSpinner.GetAvailableRanges();

        if (RangesList != null)
        {
            for (int i = 0; i < RangesList.Count; i++)
            {
                Ranges.Add(new List<double>());

                if (RangesList[i] != null)
                {
                    foreach (double val in RangesList[i])
                    {
                        Ranges[i].Add(val);
                    }
                }
            }
        }
    }

    public void BuildSampleRates()
    {
        SampleRates.Clear();

        var SampleRateList = _ballSpinner.GetAvailableSampleRates();

        if (SampleRateList != null)
        {
            for (int i = 0; i < SampleRateList.Count; i++)
            {
                SampleRates.Add(new List<double>());

                if (SampleRateList[i] != null)
                {
                    foreach (double val in SampleRateList[i])
                    {
                        SampleRates[i].Add(val);
                    }
                }
            }
        }
    }
}
