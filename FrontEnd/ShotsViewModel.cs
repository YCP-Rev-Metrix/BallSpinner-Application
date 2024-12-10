using RevMetrix.BallSpinner.BackEnd;
using Common.POCOs;
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

    public async Task UpdateCollectionContent()
    {
        Shots.Clear();
        SimulatedShotList list = await _database.GetListOfShots();
        if (list != null)
        {
            foreach (var shot in list.shots) 
            { 
                shot.simulatedShot.DataCount = shot.data.Count;
                Shots.Add(shot); 
            }
        }
    }
}
