namespace Avalonia.WYSWYG.Widgets.AdaptiveCards;

public static class AdaptiveCardWidget
{
    public static void Init(WidgetStorage storage)
    {
        storage.Widgets.Add(new()
        {
            Name = "Card",
            IconKind = MaterialIconKind.Toll,
            Template = new AdaptiveCardTemplate(),
            EditViewTemplate = new AdaptiveCardEditTemplate()
        });
    }
}
