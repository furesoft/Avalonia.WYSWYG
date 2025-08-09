using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Attributes;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph;

public abstract class InputNode : EmptyNode
{
    protected InputNode(string label) : base(label)
    {
    }

    [Pin("Flow", PinAlignment.Top)] public IInputPin InputPin { get; set; }
}
