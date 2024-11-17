using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;
internal class ShotsViewModel
{
    readonly IList<SimulatedShot> source;

    public ObservableCollection<SimulatedShot> Shots { get; private set; }

    public ShotsViewModel()
    {
        source = new List<SimulatedShot>();
        CreateShotsCollection();
    }

    private void CreateShotsCollection()
    {
        source.Add(new SimulatedShot { shot = new ShotInfo("Patrick", 20, 20, 20, 20)});

        source.Add(new SimulatedShot { shot = new ShotInfo("Ryan", 20, 20, 20, 20)});

        source.Add(new SimulatedShot { shot = new ShotInfo("Christain", 20, 20, 20, 20)});

        source.Add(new SimulatedShot { shot = new ShotInfo("LONG TEST!!!!!!!!!!!!!!!!!!!!!!!!!", 20, 20, 20, 20)});

        for (int i = 0; i < 100; i++) 
        {
            source.Add(new SimulatedShot { shot = new ShotInfo("LONGTESTSTSTSTSTSTSTSTS " + i, 20, 20, 20, 20)});
        }

        Shots = new ObservableCollection<SimulatedShot>(source);
    }

    public void UpdateTable()
    {
        Shots.Clear();
        Shots.Add(new SimulatedShot { shot = new ShotInfo("Hi", 20, 20, 20, 20)});
        Shots.Add(new SimulatedShot { shot = new ShotInfo("Hi", 20, 20, 20, 20)});
        Shots.Add(new SimulatedShot { shot = new ShotInfo("Hi", 20, 20, 20, 20)});
    }
}
