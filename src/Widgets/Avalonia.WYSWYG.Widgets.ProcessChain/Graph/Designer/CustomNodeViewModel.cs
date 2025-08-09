using NodeViewModel = Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM.NodeViewModel;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer;

public class CustomNodeViewModel : NodeViewModel
{
    public bool IsRemovable { get; set; } = true;
    public bool IsMovable { get; set; } = true;

    public string Category { get; set; }

    public EmptyNode DefiningNode { get; set; }


    public override bool CanRemove()
    {
        return IsRemovable;
    }

    public override bool CanMove()
    {
        return IsMovable;
    }
}
