using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph;

public static class IDrawingExtensions
{
    public static IEnumerable<CustomNodeViewModel> GetNodes<T>(this IDrawingNode drawing)
    {
        return drawing.Nodes
            .OfType<CustomNodeViewModel>()
            .Where(node => node.DefiningNode.GetType() == typeof(T));
    }
}
