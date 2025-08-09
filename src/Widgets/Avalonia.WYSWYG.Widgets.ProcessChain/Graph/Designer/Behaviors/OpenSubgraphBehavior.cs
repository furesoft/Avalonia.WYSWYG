using Avalonia.Interactivity;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.ViewModels;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Nodes;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.NodeViews;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Behaviors;

public class OpenSubgraphBehavior : Behavior<Control>
{
    public static readonly StyledProperty<MainViewViewModel> ViewModelProperty =
        AvaloniaProperty.Register<OpenSubgraphBehavior, MainViewViewModel>(nameof(ViewModel));

    public MainViewViewModel ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is not null)
        {
            AssociatedObject.DoubleTapped += DoubleTapped;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is not null)
        {
            AssociatedObject.DoubleTapped -= DoubleTapped;
        }
    }

    private void DoubleTapped(object sender, RoutedEventArgs args)
    {
        if (AssociatedObject is SubgraphView {DataContext: SubgraphNode node})
        {
           // ViewModel.OpenDocument(node.GraphItem);
        }
    }
}
