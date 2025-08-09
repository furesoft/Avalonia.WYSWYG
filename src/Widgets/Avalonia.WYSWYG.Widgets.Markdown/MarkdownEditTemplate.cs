using Avalonia.Controls;
using Avalonia.Data;

namespace Avalonia.WYSWYG.Widgets.Markdown;

public class MarkdownEditTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var textBox = new TextBox
        {
            MinHeight = 150,
            MinWidth = 150,
            AcceptsReturn = true,
            [!TextBox.TextProperty] = new Binding("Properties[Text]", BindingMode.TwoWay)
        };

        return textBox;
    }
}