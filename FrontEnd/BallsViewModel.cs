using RevMetrix.BallSpinner.BackEnd;
using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.ApplicationModel.Contacts;

namespace RevMetrix.BallSpinner.FrontEnd;
internal class BallsViewModel
{
    public ObservableCollection<Ball> Arsenal { get; private set; }
    public ObservableCollection<Ball> ArsenalSorted { get; private set; }
    private Arsenal list;
    private IDatabase _database;

    public BallsViewModel(IDatabase database)
    {
        Arsenal = new ObservableCollection<Ball>();
        _database = database;
        UpdateCollectionContent();
    }

    public async Task UpdateCollectionContent()
    {
        Arsenal.Clear();
        list = await _database.GetArsenal();

        if (list != null) 
        {
            foreach (var ball in list.BallList) { Arsenal.Add(ball); }
        }
    }

    public void SortCollection(int sortID)
    {
        Arsenal.Clear();

        if (list != null)
        {
            switch (sortID)
            {
                case 0:
                    list.BallList.Sort((x, y) => string.Compare(x.Name, y.Name));
                    break;
                case 1:
                    list.BallList.Sort((x, y) => string.Compare(y.Name, x.Name));
                    break;
                case 2:
                    list.BallList.Sort((x, y) => string.Compare(x.Name, y.Name));
                    list.BallList.Sort((x, y) => string.Compare(x.CoreType, y.CoreType));
                    break;
                case 3:
                    list.BallList.Sort((x, y) => string.Compare(x.Name, y.Name));
                    list.BallList.Sort((x, y) => string.Compare(y.CoreType, x.CoreType));
                    break;
                case 4:
                    list.BallList.Sort((x, y) => (int)(x.Weight - y.Weight));
                    break;
                case 5:
                    list.BallList.Sort((x, y) => (int)(y.Weight - x.Weight));
                    break;
            }

            foreach (var ball in list.BallList) { Arsenal.Add(ball); }
        }
    }
}
