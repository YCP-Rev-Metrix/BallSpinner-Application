using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;

internal class SmartDotSettingsViewModel
{
    private IBallSpinner _ballSpinner = null;
    public List<List<double>> Ranges;
    public List<List<double>> SampleRates;

    public SmartDotSettingsViewModel(IBallSpinner ballSpinner)
    {
        _ballSpinner = ballSpinner;
        Ranges = _ballSpinner.GetAvailableRanges();
        SampleRates = _ballSpinner.GetAvailableSampleRates();
    }
}
