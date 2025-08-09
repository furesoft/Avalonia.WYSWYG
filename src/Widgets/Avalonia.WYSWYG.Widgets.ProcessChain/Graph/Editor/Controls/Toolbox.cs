using Avalonia.Controls.Primitives;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Controls;

public class Toolbox : TemplatedControl
{
    public static readonly StyledProperty<ObservableCollection<object>> TemplatesProperty =
        AvaloniaProperty.Register<Toolbox, ObservableCollection<object>>(nameof(Templates));

    public ObservableCollection<object> Templates
    {
        get => GetValue(TemplatesProperty);
        set => SetValue(TemplatesProperty, value);
    }
}
