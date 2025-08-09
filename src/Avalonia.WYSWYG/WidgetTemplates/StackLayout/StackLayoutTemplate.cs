using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace Avalonia.WYSWYG.WidgetTemplates.StackLayout;

public class StackLayoutTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var itemsControl = new ItemsControl
        {
            ItemsSource = model.GetProperty<ObservableCollection<WidgetModel>>("Items"),
            ItemsPanel = new FuncTemplate<Panel>(() =>
            {
                var stackPanel = new StackPanel
                {
                    Spacing = 5, Orientation = model.GetProperty<Orientation>("Orientation")
                };

                return stackPanel;
            })
        };

        return itemsControl;
    }
}
