using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevMetrix.BallSpinner.FrontEnd;
public class BallViewModel : WebDataViewModel
{
    public BallViewModel(IBallSpinner ballSpinner) : base(ballSpinner)
    {
        
    }

    public override string Source => "http://localhost:8081/Pages/ThreeJS/ThreeJS.html";

    public override Metric Metrics => Metric.RotationX | Metric.RotationY | Metric.RotationZ;
}
