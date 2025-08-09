using Material.Icons;

namespace Avalonia.WYSWYG.Widgets.Markdown;

public static class MarkdownWidget
{
    public static void Init(WidgetStorage storage)
    {
        storage.Widgets.Add(new()
        {
            Name = "Markdown",
            IconKind = MaterialIconKind.LanguageMarkdown,
            Template = new MarkdownTemplate(),
            EditViewTemplate = new MarkdownEditTemplate()
        });
    }
}
