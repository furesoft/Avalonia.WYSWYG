using Material.Icons;

namespace Avalonia.WYSWYG.Widgets.Map;

public static class MapWidget
{
    public static void Init(WidgetStorage storage)
    {
        storage.Widgets.Add(new()
        {
            Name = "Map",
            IconKind = MaterialIconKind.Map,
            Template = new MapTemplate(),
            EditViewTemplate = new MapEditTemplate()
        });
    }
}
