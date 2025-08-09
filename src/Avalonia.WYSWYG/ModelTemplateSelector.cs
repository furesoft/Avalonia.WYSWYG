using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Avalonia.WYSWYG;

public class ModelTemplateSelector : IDataTemplate
{
    public Control Build(object param)
    {
        var model = (WidgetModel)param;

        return model.Template.Build(model);
    }

    public bool Match(object data)
    {
        var model = (WidgetModel)data;

        return model!.Template != null;
    }
}
