using Common.POCOs;
using RevMetrix.BallSpinner.BackEnd.BallSpinner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.FrontEnd;
public partial class BallSpinnerViewModel : INotifyPropertyChanged
{
    public string Name 
    {
        get => _ballSpinner.Name;
        set
        {
            _ballSpinner.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private IBallSpinner _ballSpinner;

    public IDataViewModel LeftView { get; }

    public IDataViewModel TopMiddleView { get; }

    public IDataViewModel BottomMiddleView { get; }

    public IDataViewModel TopRightView { get; }

    public IDataViewModel BottomRightView { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public BallSpinnerViewModel(IBallSpinner ballspinner)
    {
        _ballSpinner = ballspinner;

        LeftView = new BallViewModel(_ballSpinner);
        TopMiddleView = new GraphViewModel(_ballSpinner);
        BottomMiddleView = new GraphViewModel(_ballSpinner);
        TopRightView = new GraphViewModel(_ballSpinner);
        BottomRightView = new GraphViewModel(_ballSpinner);
    }

    public void Start()
    {
        _ballSpinner.Start();
    }


    public void Stop()
    {
        _ballSpinner.Stop();
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
