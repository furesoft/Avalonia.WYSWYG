using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Attributes;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph;

public abstract class OutputNode : EmptyNode
{
    protected OutputNode(string label) : base(label)
    {
    }

    [Pin("Flow", PinAlignment.Bottom)] public IOutputPin OutputPin { get; set; }
}
