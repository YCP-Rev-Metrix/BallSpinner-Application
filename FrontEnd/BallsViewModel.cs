using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
using RevMetrix.BallSpinner.BackEnd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;
internal class BallsViewModel
{
    public ObservableCollection<Balls> Balls { get; private set; }
    private IDatabase _database = null;

    public BallsViewModel(IDatabase database)
    {
        Balls = new ObservableCollection<Balls>();
        _database = database;
        UpdateCollectionContent();
    }

    // The 
    public async void UpdateCollectionContent()
    {
        Balls.Clear();
        //Balls.Add()
    }
}
