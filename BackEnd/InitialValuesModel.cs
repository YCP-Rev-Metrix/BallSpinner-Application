using System.Net.Http.Headers;

namespace RevMetrix.BallSpinner.BackEnd;

public class InitialValuesModel
{
    public InitialValuesModel()
    {
        
    }

    public List<List<double>> CalculateBezierCurve(Coordinates pointInit, Coordinates inflectPoint, Coordinates pointFinal)

    {
        //First List: Linear Treadline X
        //Second List: Linear Treadline Y
        //Thirst List: Bezier Curve Treadline X
        //Fourth List: Bezier Curve Treadline Y
        List<List<double>> result = new List<List<double>>();
        // Put in the algorithm stuff baby!!!!!!!!!! WOOOOOOOOOO!!!!!!!!!!!
        List<double> t = new List<double>();
        List<double> x = new List<double>();
        List<double> y = new List<double>();

        // generate t (which is also resolution of curve)
        for (double i = 0; i <= 1; i += 0.003)
        {
            //Assume x = t = 100s
            x.Add(i*100);
            //Y from 0 to 800
            y.Add(i *800);
            t.Add(i);
        }

        result.Add(x);
        result.Add(y);
        
        //Adds Points of line to 

        //Hardcoded to generate data for testing
        double P1x = 0  /*pointInit.x*/, P1y = 0 /*pointInit.y*/;
        double P3x = 70 /*inflectPoint.x*/, P3y = 50 /*inflectPoint.y*/;
        
        //Hardcoded to generate data for testing
        double P5x = 100, P5y = 800;

        double P2x = (P3x - P1x) / 2;
        double P2y = ((P3y - P1y) / (P3x - P1x)) * P2x + P1y;

        double P4x = ((P5x - P3x) / 2) + P3x;
        double P4y = ((P5y - P3y) / (P5x - P3x)) * (P4x - P3x) + P3y;

        List<double> BezierXAxis = new List<double>(); 
        List<double> BezierYAxis = new List<double>();

        //Generate Curve
        foreach (double ti in t)
        {
            // Run all Exponentials 
            double oneMinusT = 1 - ti;
            double tFourth = Math.Pow(oneMinusT, 4);
            double tThird = Math.Pow(oneMinusT, 3);
            double tSecond = Math.Pow(oneMinusT, 2);

            BezierXAxis.Add(tFourth* P1x +
                    4 * tThird * ti * P2x +
                    6 * tSecond * Math.Pow(ti, 2) * P3x +
                    4 * oneMinusT * Math.Pow(ti, 3) * P4x +
                    Math.Pow(ti, 4) * P5x);

            BezierYAxis.Add(tFourth * P1y +
                    4 * tThird * ti * P2y +
                    6 * Math.Pow(oneMinusT, 2) * Math.Pow(ti, 2) * P3y +
                    4 * oneMinusT * Math.Pow(ti, 3) * P4y +
                    Math.Pow(ti, 4) * P5y);
        }

        result.Add(BezierXAxis);
        result.Add(BezierYAxis);
        return result;
    }
}

public class Coordinates
{
    public double x { get; set; }
    public double y { get; set; }


    public Coordinates(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
}