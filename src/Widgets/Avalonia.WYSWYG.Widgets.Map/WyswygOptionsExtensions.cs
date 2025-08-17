using Material.Icons;

namespace Avalonia.WYSWYG.Widgets.Map;

public static class WyswygOptionsExtensions
{
    public static void AddMap(this WyswygOptions options)
    {
        options.AddWidget(new()
        {
            Name = "Map",
            IconKind = MaterialIconKind.Map,
            Template = new MapTemplate(),
            EditViewTemplate = new MapEditTemplate()
        });
    }
}