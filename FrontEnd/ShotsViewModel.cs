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
    //readonly IList<SimulatedShot> source;
    public ObservableCollection<SimulatedShot> Shots { get; private set; }

    public ShotsViewModel()
    {
        //source = new List<SimulatedShot>();
        Shots = new ObservableCollection<SimulatedShot>();
        BuildShotsCollection();
    }

    public void BuildShotsCollection()
    {
        int rand = 0;
        if(rand == 0)
        {
            Shots.Add(new SimulatedShot { Name = "Patrick", DateSaved = "10/21/2002" });
            Shots.Add(new SimulatedShot { Name = "Ryan", DateSaved = "11/13/2002" });
            Shots.Add(new SimulatedShot { Name = "Christian", DateSaved = "12/28/2002" });
            Shots.Add(new SimulatedShot { Name = "This is a long name test, lets see how this works", DateSaved = "01/01/0001" });

            for (int i = 0; i < 100; i++)
            {
                Shots.Add(new SimulatedShot { Name = "This is a many name test: " + i, DateSaved = "1/1/0" + i });
            }
        } else 
        {
            Shots.Clear();
            Shots.Add(new SimulatedShot { Name = "Chris", DateSaved = "6/21/2003" });
            Shots.Add(new SimulatedShot { Name = "Chris 2", DateSaved = "3/11/2002" });
            Shots.Add(new SimulatedShot { Name = "Kensie", DateSaved = "11/22/2002" });
        }
    }
}
