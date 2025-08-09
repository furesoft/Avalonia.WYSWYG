using Avalonia.Controls.Primitives;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Controls;

public class DrawingNodeProperties : TemplatedControl
{
    public static readonly StyledProperty<DrawingNode> DrawingNodeProperty =
        AvaloniaProperty.Register<DrawingNodeProperties, DrawingNode>(nameof(DrawingNode));

    public DrawingNode DrawingNode
    {
        get => GetValue(DrawingNodeProperty);
        set => SetValue(DrawingNodeProperty, value);
    }
}
