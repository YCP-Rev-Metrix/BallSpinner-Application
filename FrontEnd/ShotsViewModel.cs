﻿using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
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
        source.Add(new SimulatedShot { Name = "Patrick", DateSaved = "10/21/2002" });

        source.Add(new SimulatedShot { Name = "Ryan", DateSaved = "11/13/2002" });

        source.Add(new SimulatedShot { Name = "Christian", DateSaved = "12/28/2002" });

        source.Add(new SimulatedShot { Name = "This is a long name test, lets see how this works", DateSaved = "01/01/0001" });

        for (int i = 0; i < 100; i++) 
        {
            source.Add(new SimulatedShot { Name = "This is a many name test: " + i, DateSaved = "1/1/0" + i });
        }

        Shots = new ObservableCollection<SimulatedShot>(source);
    }
}
