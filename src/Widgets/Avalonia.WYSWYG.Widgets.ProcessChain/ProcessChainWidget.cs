namespace Avalonia.WYSWYG.Widgets.ProcessChain;

public static class ProcessChainWidget
{
    public static void Init(WidgetStorage storage)
    {
        storage.Widgets.Add(new()
        {
            Name = "Process Chain",
            IconKind = MaterialIconKind.Graph,
            // Template = new MarkdownTemplate(),
            EditViewTemplate = new ProcessChainEditTemplate()
        });
    }
}
