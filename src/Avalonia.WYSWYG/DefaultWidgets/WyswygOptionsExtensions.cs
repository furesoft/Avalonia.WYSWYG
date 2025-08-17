using Avalonia.WYSWYG.WidgetTemplates.Link;
using Avalonia.WYSWYG.WidgetTemplates.StackLayout;
using Material.Icons;

namespace Avalonia.WYSWYG.DefaultWidgets;

public static class WyswygOptionsExtensions
{
    public static void AddDefaults(this WyswygOptions options)
    {
        options.AddWidget(new()
        {
            DisplayName = "Link",
            IconKind = MaterialIconKind.Link,
            Template = new LinkTemplate(),
            EditViewTemplate = new LinkEditTemplate(),
            Properties =
            {
                ["URL"] = "",
                ["Label"] = "Link",
            }
        });
        options.AddWidget(new()
        {
            DisplayName = "StackLayout",
            IconKind = MaterialIconKind.ListBox,
            Template = new StackLayoutTemplate(),
            EditViewTemplate = new StackLayoutEditTemplate()
        });
    }
}
