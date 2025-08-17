using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.WYSWYG.Parsing;
using Splat;

namespace Avalonia.WYSWYG.Controls;

public class WidgetSearch : TemplatedControl
{
    private AutoCompleteBox _box;
    private Grid? _container;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _box = e.NameScope.Find<AutoCompleteBox>("PART_box")!;
        _box.ItemFilter = ItemFilter;
        _box.TextSelector = (search, item) => '/' + item;
        _box.KeyDown += BoxOnKeyDown;

        _container = e.NameScope.Find<Grid>("PART_container");
    }

    private void BoxOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var widgets = Locator.Current.GetService<WidgetStorage>();
            var instance = widgets!.Instanciate(Block.Parse(_box.Text!));

            //replace search with instance
            _container.Children.Clear();
            var host = new ContentControl();
            host.Content = instance;
            _container.Children.Add(host);
        }
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