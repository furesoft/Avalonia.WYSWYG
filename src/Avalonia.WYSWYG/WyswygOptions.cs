namespace Avalonia.WYSWYG;

public class WyswygOptions
{
    internal readonly WidgetStorage WidgetStorage = new();

    public void AddWidget(WidgetModel model)
    {
        WidgetStorage.Widgets.Add(model);
    }
}