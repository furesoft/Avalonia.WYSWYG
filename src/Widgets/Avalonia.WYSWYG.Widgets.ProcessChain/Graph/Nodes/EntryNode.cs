using System.ComponentModel;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Attributes;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.NodeViews;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Nodes;

[IgnoreTemplate]
[NodeView(typeof(EntryView))]
[Description("The starting node of the graph")]
public class EntryNode : OutputNode
{
    public EntryNode() : base("Entry")
    {
    }

    [Browsable(false)] public new bool ShowDescription { get; set; }
}
