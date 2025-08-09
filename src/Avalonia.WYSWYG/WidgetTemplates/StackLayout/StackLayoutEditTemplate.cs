using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.WYSWYG.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.WYSWYG.WidgetTemplates.StackLayout;

public class StackLayoutEditTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var items = model.GetProperty<ObservableCollection<WidgetModel>>("Items") ?? new();

        var deleteCommand = new RelayCommand<WidgetModel>(mm =>
        {
            var itemsList = items.ToList();
            var index = items.IndexOf(mm);

            itemsList.RemoveAt(index);

            model.SetProperty("Items", new ObservableCollection<WidgetModel>(itemsList));
        });

        var stackPanel = new StackPanel {Orientation = model.GetProperty<Orientation>("Orientation"), Spacing = 5};

        foreach (var block in items)
        {
            stackPanel.Children.Add(new EditableContainer
            {
                EditModeContent = block.EditViewTemplate != null ? block.EditViewTemplate.Build(block) : block,
                Content = block.EditViewTemplate != null ? block.EditViewTemplate.Build(block) : block,
                CommandParameter = block,
                IsEditMode = true,
                DeleteCommand = deleteCommand
            });
        }

        var orientationComboBox = new ComboBox();

        orientationComboBox.Items.Add(Orientation.Horizontal);
        orientationComboBox.Items.Add(Orientation.Vertical);

        orientationComboBox.SelectedItem = stackPanel.Orientation;
        orientationComboBox.SelectionChanged +=
            (_, _) => model.SetProperty("Orientation", orientationComboBox.SelectedItem);

        var root = new StackPanel();
        root.Children.Add(orientationComboBox);
        root.Children.Add(stackPanel);

        return new Border {Child = root, BorderBrush = Brushes.Black, BorderThickness = new(1), Padding = new(5)};
    }
}
