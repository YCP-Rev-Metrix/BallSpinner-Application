using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;
public class GraphViewModel : WebDataViewModel
{
    public string Name { get; }

    public GraphViewModel(IBallSpinner ballSpinner, string name, Metric metrics) : base(ballSpinner)
    {
        Name = name;
        Metrics = metrics;
    }

    public override string Source => "http://localhost:8081/Pages/ChartJS/ChartJS.html";

    public override Metric Metrics { get; }
}
