using Avalonia.Controls;

namespace Avalonia.WYSWYG;

public class FuncWidgetTemplate : WidgetTemplate
{
    private readonly Func<WidgetModel, Control> _func;

    public FuncWidgetTemplate(Func<WidgetModel, Control> func)
    {
        _func = func;
    }

    public override Control Build(WidgetModel model)
    {
        return _func(model);
    }
}
