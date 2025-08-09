namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

public interface INodeFactory
{
    IList<INodeTemplate> CreateTemplates();
    IDrawingNode CreateDrawing(string name = null);
}
