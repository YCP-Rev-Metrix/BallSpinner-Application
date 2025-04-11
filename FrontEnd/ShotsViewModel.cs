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
    SimulatedShotList list;
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
        list = await _database.GetListOfShots();

        if (list != null)
        {
            foreach (var shot in list.shots)
            {
                shot.shotinfo.DataCount = shot.data.Count;
                Shots.Add(shot);
            }
        }
    }

    public void SortCollection(int sortID)
    {
        Shots.Clear();
        
        if (list != null)
        {
            switch (sortID)
            {
                case 0:
                    //list.Sort((x, y) => string.Compare(x.Date, y.Date));
                    break;
                case 1:
                    //list.Sort((x, y) => string.Compare(y.Date, x.Date));
                    break;
                case 2:
                    list.shots.Sort((x, y) => string.Compare(x.shotinfo.Name, y.shotinfo.Name));
                    break;
                case 3:
                    list.shots.Sort((x, y) => string.Compare(y.shotinfo.Name, x.shotinfo.Name));
                    break;
                case 4:
                    list.shots.Sort((x, y) => string.Compare(x.shotinfo.Name, y.shotinfo.Name));
                    list.shots.Sort((x, y) => (int)(x.shotinfo.DataCount - y.shotinfo.DataCount));
                    break;
                case 5:
                    list.shots.Sort((x, y) => string.Compare(x.shotinfo.Name, y.shotinfo.Name));
                    list.shots.Sort((x, y) => (int)(y.shotinfo.DataCount - x.shotinfo.DataCount));
                    break;
            }

            foreach (var shot in list.shots) { Shots.Add(shot); }
        }
    }
}
