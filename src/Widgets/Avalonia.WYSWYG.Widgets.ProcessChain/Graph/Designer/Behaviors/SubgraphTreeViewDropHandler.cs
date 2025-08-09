using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Behaviors;

public class SubgraphTreeViewDropHandler : DropHandlerBase
{
    public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext,
        object state)
    {
        if (sender is TreeView tv)
        {
            tv.SelectedItem = null;
        }

        return sourceContext is not null;
    }
}
