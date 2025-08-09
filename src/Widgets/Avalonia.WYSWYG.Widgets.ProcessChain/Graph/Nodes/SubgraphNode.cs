using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Attributes;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.NodeViews;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Nodes;

[IgnoreTemplate]
[NodeView(typeof(SubgraphView))]
public class SubgraphNode : DynamicNode
{
    /*
    public SubgraphNode(GraphItem graphItem) : base(null)
    {
        GraphItem = graphItem;
        Description = graphItem.Name;
        Label = graphItem.Name;

        AddPin("Input Flow", PinAlignment.Top, PinMode.Input);
        AddPin("Output Flow", PinAlignment.Bottom, PinMode.Output);
    }

    public GraphItem GraphItem { get; set; }
   */
    public SubgraphNode(string label, Control view = null) : base(label, view)
    {
    }
}
