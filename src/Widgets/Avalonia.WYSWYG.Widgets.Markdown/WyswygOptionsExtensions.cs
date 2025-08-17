using Material.Icons;

namespace Avalonia.WYSWYG.Widgets.Markdown;

public static class WyswygOptionsExtensions
{
    public static void AddMarkdown(this WyswygOptions options)
    {
        options.AddWidget(new()
        {
            DisplayName = "Markdown",
            IconKind = MaterialIconKind.LanguageMarkdown,
            Template = new MarkdownTemplate(),
            EditViewTemplate = new MarkdownEditTemplate()
        });
    }
}
