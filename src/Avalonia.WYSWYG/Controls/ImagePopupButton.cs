using System.Windows.Input;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace Avalonia.WYSWYG.Controls;

public class ImagePopupButton : ToggleButton
{
    public static readonly StyledProperty<MaterialIconKind> KindProperty =
        AvaloniaProperty.Register<ImagePopupButton, MaterialIconKind>(nameof(Kind));

    public static readonly StyledProperty<object> PopupContentProperty =
        AvaloniaProperty.Register<ImagePopupButton, object>(nameof(PopupContent));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<ImagePopupButton, string>(nameof(TextProperty));

    public new Type StyleKeyOverride => typeof(ImagePopupButton);

    public ImagePopupButton()
    {
        OpenPopupCommand = new RelayCommand(Toggle);
    }

    public ICommand OpenPopupCommand { get; set; }

    public MaterialIconKind Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public object PopupContent
    {
        get => GetValue(PopupContentProperty);
        set => SetValue(PopupContentProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public new void Toggle()
    {
        IsChecked = !IsChecked;
    }
}
