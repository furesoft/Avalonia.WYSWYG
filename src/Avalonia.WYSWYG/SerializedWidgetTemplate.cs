using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.WYSWYG;

public class SerializedWidgetTemplate(string xaml) : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        return (Control)AvaloniaRuntimeXamlLoader.Load(xaml);
    }
}
