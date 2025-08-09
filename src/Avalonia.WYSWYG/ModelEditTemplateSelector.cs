using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Avalonia.WYSWYG;

public class ModelEditTemplateSelector : IDataTemplate
{
    public Control Build(object param)
    {
        var model = (WidgetModel)param;

        return model.EditViewTemplate.Build(model);
    }

    public bool Match(object data)
    {
        return data is WidgetModel model && model!.EditViewTemplate != null;
    }
}
