using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Avalonia.WYSWYG;

public abstract class WidgetTemplate : IDataTemplate
{
    public bool Match(object data)
    {
        return data is WidgetModel;
    }

    public Control Build(object param)
    {
        return Build(param as WidgetModel);
    }

    public abstract Control Build(WidgetModel model);
}
