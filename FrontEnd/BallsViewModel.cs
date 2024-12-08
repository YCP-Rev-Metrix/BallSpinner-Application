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
internal class BallsViewModel
{
    public ObservableCollection<Ball> Arsenal { get; private set; }
    private IDatabase _database;

    public BallsViewModel(IDatabase database)
    {
        Arsenal = new ObservableCollection<Ball>();
        _database = database;
        UpdateCollectionContent();
    }

    public void UpdateCollectionContent()
    {
        Arsenal.Clear();
        /* Arsenal list = _database.GetArsenal();
        if (list != null)
        {
            foreach (var ball in list.BallList) { Arsenal.Add(ball); }
        } */
    }
}
