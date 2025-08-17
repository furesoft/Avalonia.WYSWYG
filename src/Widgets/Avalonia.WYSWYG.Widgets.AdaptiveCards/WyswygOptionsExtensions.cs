using Material.Icons;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards;

public static class WyswygOptionsExtensions
{
    public static void AddAdaptiveCard(this WyswygOptions options)
    {
        options.AddWidget(new()
        {
            Name = "Card",
            IconKind = MaterialIconKind.Toll,
            Template = new AdaptiveCardTemplate(),
            EditViewTemplate = new AdaptiveCardEditTemplate()
        });
    }
}