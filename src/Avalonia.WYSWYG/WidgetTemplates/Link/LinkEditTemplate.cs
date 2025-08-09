using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalonia.WYSWYG.WidgetTemplates.Link;

public class LinkEditTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var stackPanel = new StackPanel {Spacing = 2, Orientation = Orientation.Horizontal};

        var labelTb = new TextBox
        {
            Watermark = "Label",
            UseFloatingWatermark = true,
            [!TextBox.TextProperty] = new Binding("Properties[Label]", BindingMode.TwoWay),
            FontSize = 15,
            MinWidth = 150
        };

        stackPanel.Children.Add(labelTb);

        var urlTb = new TextBox
        {
            Watermark = "URL",
            UseFloatingWatermark = true,
            [!TextBox.TextProperty] = new Binding("Properties[URL]", BindingMode.TwoWay),
            FontSize = 15,
            MinWidth = 150
        };

        stackPanel.Children.Add(urlTb);

        return new Border {Child = stackPanel, BorderBrush = Brushes.Black, BorderThickness = new(1), Padding = new(2)};
    }
}
