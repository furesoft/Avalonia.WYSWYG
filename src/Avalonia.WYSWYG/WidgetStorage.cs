using Avalonia.WYSWYG.Parsing;
using Avalonia.WYSWYG.WidgetTemplates.Link;
using Avalonia.WYSWYG.WidgetTemplates.StackLayout;
using Material.Icons;

namespace Avalonia.WYSWYG;

public class WidgetStorage
{
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
            DisplayName = model.DisplayName,
            OnAddCommand = model.OnAddCommand,
            Extractor = model.Extractor,
            Properties = model.Properties,
        };
    }

    public WidgetModel Instanciate(Block block)
    {
        var model = Widgets.First(model => model.Command == block.Name);

        var instance = Instanciate(model.ID);

        foreach (var prop in block.Properties)
        {
            instance.Properties[prop.Key] = prop.Value;
        }

        return instance;
    }

    public WidgetModel? FindByName(string name)
    {
        return Widgets.FirstOrDefault(model => model.DisplayName == name);
    }
}
