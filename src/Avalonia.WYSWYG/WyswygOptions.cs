namespace Avalonia.WYSWYG;

public class WyswygOptions
{
    internal readonly WidgetStorage WidgetStorage = new();

    public void AddWidget(WidgetModel model)
    {
        model.Command ??= model.DisplayName.ToLower();

        WidgetStorage.Widgets.Add(model);
    }
}