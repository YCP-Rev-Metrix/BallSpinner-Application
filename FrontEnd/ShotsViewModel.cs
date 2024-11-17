using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;
internal class ShotsViewModel
{
    public ObservableCollection<SimulatedShot> Shots { get; private set; }
    private IDatabase _database = null;

    public ShotsViewModel(IDatabase database)
    {
        Shots = new ObservableCollection<SimulatedShot>();
        _database = database;
        UpdateCollectionContent();
    }

    // The 
    public async Task<bool> UpdateCollectionContent()
    {
        Shots.Clear();
        Random random = new Random();
        int a = random.Next(0,4);
        
        if(a == 0)
        {
            Shots.Add(new SimulatedShot { shot = new ShotInfo("Patrick", 20, 20, 20, 20) });
            Shots.Add(new SimulatedShot { shot = new ShotInfo("Ryan", 20, 20, 20, 20) });
            Shots.Add(new SimulatedShot { shot = new ShotInfo("Christain", 20, 20, 20, 20) });
        } 
        else if(a == 1)
        {
            for (int i = 0; i < 100; i++)
            {
                Shots.Add(new SimulatedShot { shot = new ShotInfo("Many entry test: " + i, 20, 20, 20, 20) });
            }
        } 
        else if (a == 2)
        {

            Shots.Add(new SimulatedShot { shot = new ShotInfo("Long entry name test: This is a very long name, Walton Alouicious Gonzaga XV", 20, 20, 20, 20) });
        }
        else if (a == 3)
        {
            SimulatedShotList list = await _database.GetListOfShots();
            if (list != null)
            {
                foreach (var shot in list.shotList) {Shots.Add(shot);}
            }
        }

        return true;
    }
}
