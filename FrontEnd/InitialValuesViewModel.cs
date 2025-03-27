using RevMetrix.BallSpinner.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using Common.POCOs;
using System.Collections.ObjectModel;

namespace RevMetrix.BallSpinner.FrontEnd;

class InitialValuesViewModel
{
    BallsViewModel _ballsViewModel;
    InitialValuesChart chart = null;
    public ISeries[] Series { get; private set; }
    public ObservableCollection<Ball> Arsenal { get; private set; }


    public InitialValuesViewModel(IDatabase database)
    {
        _ballsViewModel = new BallsViewModel(database);
        Arsenal = _ballsViewModel.Arsenal;
        InitGraph();
    }

    private void InitGraph()
    {
        InitialValuesModel model = new InitialValuesModel();
        Coordinates dummyvalues = new Coordinates(0, 0);
        List<List<double>> axes = model.CalcuateBezierCruve(dummyvalues, dummyvalues, dummyvalues);
        chart = new InitialValuesChart(axes[0], axes[1], axes[2], axes[3]);
        Series = chart.Series;
    }
}
