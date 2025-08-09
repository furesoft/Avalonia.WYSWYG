using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM;

public class DrawingNodeFactory : IDrawingNodeFactory
{
    public static readonly IDrawingNodeFactory Instance = new DrawingNodeFactory();

    public IPin CreatePin()
    {
        return new PinViewModel();
    }

    public IConnector CreateConnector()
    {
        return new ConnectorViewModel();
    }

    public IList<T> CreateList<T>()
    {
        return new ObservableCollection<T>();
    }
}
