namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

public interface IEditor
{
    ObservableCollection<object> Templates { get; set; }
    IDrawingNode Drawing { get; set; }
}
