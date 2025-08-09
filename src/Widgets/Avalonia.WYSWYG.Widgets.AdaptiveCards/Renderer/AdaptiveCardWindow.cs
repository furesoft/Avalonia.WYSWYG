using Avalonia.Controls;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

/// <summary>
///     Class for showing a card in a window
/// </summary>
public class AdaptiveCardWindow : Window
{
    public AdaptiveCardView CardView
    {
        get => Content as AdaptiveCardView;
        set => Content = value;
    }
}