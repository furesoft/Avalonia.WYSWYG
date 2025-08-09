using System.Reactive.Linq;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;
using ReactiveMarbles.PropertyChanged;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM;

[ObservableObject]
public partial class ConnectorViewModel : IConnector
{
    [ObservableProperty] private IPin _end;
    [ObservableProperty] private string _name;
    [ObservableProperty] private double _offset = 30;
    [ObservableProperty] private ConnectorOrientation _orientation;
    [ObservableProperty] private IDrawingNode _parent;
    [ObservableProperty] private IPin _start;

    public ConnectorViewModel()
    {
        ObservePins();
    }

    public event EventHandler<ConnectorCreatedEventArgs> Created;

    public event EventHandler<ConnectorRemovedEventArgs> Removed;

    public event EventHandler<ConnectorSelectedEventArgs> Selected;

    public event EventHandler<ConnectorDeselectedEventArgs> Deselected;

    public event EventHandler<ConnectorStartChangedEventArgs> StartChanged;

    public event EventHandler<ConnectorEndChangedEventArgs> EndChanged;

    public virtual bool CanSelect()
    {
        return true;
    }

    public virtual bool CanRemove()
    {
        return true;
    }

    public void OnCreated()
    {
        Created?.Invoke(this, new(this));
    }

    public void OnRemoved()
    {
        Removed?.Invoke(this, new(this));
    }

    public void OnSelected()
    {
        Selected?.Invoke(this, new(this));
    }

    public void OnDeselected()
    {
        Deselected?.Invoke(this, new(this));
    }

    public void OnStartChanged()
    {
        StartChanged?.Invoke(this, new(this));
    }

    public void OnEndChanged()
    {
        EndChanged?.Invoke(this, new(this));
    }

    private void ObservePins()
    {
        this.WhenChanged<ConnectorViewModel, IPin>(x => x.Start)
            .DistinctUntilChanged()
            .Subscribe(start =>
            {
                if (start?.Parent is not null)
                {
                    (start.Parent as NodeViewModel)?.WhenChanged<NodeViewModel, double>(x => x.X).DistinctUntilChanged()
                        .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.Start)));
                    (start.Parent as NodeViewModel)?.WhenChanged<NodeViewModel, double>(x => x.Y).DistinctUntilChanged()
                        .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.Start)));
                }
                else
                {
                    if (start is not null)
                    {
                        (start as PinViewModel)?.WhenChanged<PinViewModel, double>(x => x.X).DistinctUntilChanged()
                            .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.Start)));
                        (start as PinViewModel)?.WhenChanged<PinViewModel, double>(x => x.Y).DistinctUntilChanged()
                            .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.Start)));
                    }
                }

                (start as PinViewModel)?.WhenChanged<PinViewModel, PinAlignment>(x => x.Alignment).DistinctUntilChanged()
                    .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.Start)));
            });

        this.WhenChanged<ConnectorViewModel, IPin>(x => x.End)
            .DistinctUntilChanged()
            .Subscribe(end =>
            {
                if (end?.Parent is not null)
                {
                    (end.Parent as NodeViewModel)?.WhenChanged<NodeViewModel, double>(x => x.X).DistinctUntilChanged()
                        .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.End)));
                    (end.Parent as NodeViewModel)?.WhenChanged<NodeViewModel, double>(x => x.Y).DistinctUntilChanged()
                        .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.End)));
                }
                else
                {
                    if (end is not null)
                    {
                        (end as PinViewModel)?.WhenChanged<PinViewModel, double>(x => x.X).DistinctUntilChanged()
                            .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.End)));
                        (end as PinViewModel)?.WhenChanged<PinViewModel, double>(x => x.Y).DistinctUntilChanged()
                            .Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.End)));
                    }
                }

                if (end is not null)
                {
                    (end as PinViewModel)?.WhenChanged<PinViewModel, PinAlignment>(x => x.Alignment).Subscribe(_ => OnPropertyChanged(nameof(Furesoft.LowCode.Editor.MVVM.ConnectorViewModel.End)));
                }
            });
    }
}
