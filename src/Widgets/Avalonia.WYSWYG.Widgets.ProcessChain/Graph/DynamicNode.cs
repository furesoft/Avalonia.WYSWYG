using System.ComponentModel;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Layout;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph;

public class DynamicNode : EmptyNode
{
    public readonly List<PinDescriptor> Pins = new();

    public DynamicNode(string label, Control view = null) : base(label)
    {
        View = view;
    }

    [Browsable(false)] public Control View { get; set; }

    public void AddPin(string name, PinAlignment alignment, PinMode mode, bool multipleConnections = false)
    {
        Pins.Add(new(name, mode, alignment, multipleConnections));
    }
}
