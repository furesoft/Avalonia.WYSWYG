using Avalonia.WYSWYG.WidgetTemplates.Link;
using Avalonia.WYSWYG.WidgetTemplates.StackLayout;
using Material.Icons;

namespace Avalonia.WYSWYG;

public class WidgetStorage
{
    public WidgetStorage()
    {
        Widgets.Add(new()
        {
            Name = "Link",
            IconKind = MaterialIconKind.Link,
            Template = new LinkTemplate(),
            EditViewTemplate = new LinkEditTemplate()
        });
        Widgets.Add(new()
        {
            Name = "StackLayout",
            IconKind = MaterialIconKind.ListBox,
            Template = new StackLayoutTemplate(),
            EditViewTemplate = new StackLayoutEditTemplate()
        });
    }

    public List<WidgetModel> Widgets { get; } = [];

    public WidgetModel Instanciate(Guid id)
    {
        var model = Widgets.First(model => model.ID == id);

        return new()
        {
            Template = model.Template,
            EditViewTemplate = model.EditViewTemplate,
            ID = model.ID,
            IconKind = model.IconKind,
            Name = model.Name,
            OnAddCommand = model.OnAddCommand,
            Extractor = model.Extractor,
            Properties = model.Properties,
        };
    }

    public WidgetModel? FindByName(string name)
    {
        return Widgets.FirstOrDefault(model => model.Name == name);
    }
}
