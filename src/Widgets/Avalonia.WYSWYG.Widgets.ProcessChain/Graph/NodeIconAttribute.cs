using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.NodeViews;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph;

[AttributeUsage(AttributeTargets.Class)]
public class NodeIconAttribute : NodeViewAttribute
{
    public NodeIconAttribute(string svgPath) : base(typeof(IconNodeView), svgPath)
    {
    }
}
