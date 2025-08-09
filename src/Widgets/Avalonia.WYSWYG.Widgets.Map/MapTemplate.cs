using Avalonia.Controls;
using Mapsui.Extensions;
using Mapsui.Tiling;
using Mapsui.UI.Avalonia;
using Mapsui.Widgets.Zoom;

namespace Avalonia.WYSWYG.Widgets.Map;

public class MapTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var map = new MapControl();
        map.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Map.Widgets.Add(new ZoomInOutWidget());
        map.Height = 500;
        map.Width = 500;

        return map;
    }
}