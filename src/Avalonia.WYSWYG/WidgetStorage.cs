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

    public WidgetModel? FindByName(string name)
    {
        return Widgets.FirstOrDefault(model => model.DisplayName == name);
    }
}
