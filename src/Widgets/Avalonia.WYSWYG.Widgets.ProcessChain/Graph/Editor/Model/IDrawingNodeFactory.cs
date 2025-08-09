namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

public interface IDrawingNodeFactory
{
    IPin CreatePin();
    IConnector CreateConnector();
    public IList<T> CreateList<T>();
}
