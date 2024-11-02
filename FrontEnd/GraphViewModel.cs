using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;
public class GraphViewModel : WebDataViewModel
{
    public GraphViewModel(IBallSpinner ballSpinner) : base(ballSpinner)
    {
    }

    public override string Source => "http://localhost:8081/Pages/ChartJS/ChartJS.html";

    public override Metric Metrics { get; }
}
