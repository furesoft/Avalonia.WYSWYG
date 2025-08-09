using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.WYSWYG.Controls;

public class WidgetDisplay : TemplatedControl
{
    public static readonly StyledProperty<bool> IsEditModeProperty =
        AvaloniaProperty.Register<WidgetDisplay, bool>(nameof(IsEditMode));

    public static readonly StyledProperty<AvaloniaList<WidgetModel>> ItemsProperty =
        AvaloniaProperty.Register<WidgetDisplay, AvaloniaList<WidgetModel>>(nameof(Items));

    public static readonly StyledProperty<ICommand> DeleteCommandProperty =
        AvaloniaProperty.Register<EditableContainer, ICommand>(nameof(DeleteCommand));

    public WidgetDisplay()
    {
        Items = new();
        DeleteCommand = new RelayCommand<WidgetModel>(Delete);
    }

    public AvaloniaList<WidgetModel> Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public bool IsEditMode
    {
        get => GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    private void Delete(WidgetModel obj)
    {
        Items.Remove(obj);
    }
}
