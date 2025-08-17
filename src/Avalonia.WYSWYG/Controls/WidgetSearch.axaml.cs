using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace Avalonia.WYSWYG.Controls;

public class WidgetSearch : TemplatedControl
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var box = e.NameScope.Find<AutoCompleteBox>("PART_box");
        box.ItemFilter = ItemFilter;
        box.TextSelector = (search, item) => '/' + item;
    }

    private bool ItemFilter(string? search, object? item)
    {
        if (!search!.StartsWith('/'))
        {
            return false;
        }

        if (item is WidgetModel model)
        {
            if (model.Command!.Contains(search[1..]))
            {
                return true;
            }
        }

        return false;
    }
}