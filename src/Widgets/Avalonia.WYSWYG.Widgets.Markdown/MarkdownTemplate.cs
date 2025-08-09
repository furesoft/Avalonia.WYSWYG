using Avalonia.Controls;
using Avalonia.Data;
using Markdown.Avalonia.Full;
using Markdown.Avalonia.SyntaxHigh;

namespace Avalonia.WYSWYG.Widgets.Markdown;

public class MarkdownTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var md = new MarkdownScrollViewer
        {
            [!global::Markdown.Avalonia.MarkdownScrollViewer.MarkdownProperty] =
                new Binding("Properties[Text]", BindingMode.TwoWay)
        };

        md.Plugins.Plugins.Add(new SyntaxHighlight());

        return md;
    }
}