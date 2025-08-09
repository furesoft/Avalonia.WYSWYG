using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.WYSWYG.Controls;

public class EditableContainer : ContentControl
{
    public static readonly StyledProperty<bool> IsEditModeProperty =
        AvaloniaProperty.Register<EditableContainer, bool>(nameof(IsEditMode));

    public static readonly StyledProperty<bool> IsDeleteableProperty =
        AvaloniaProperty.Register<EditableContainer, bool>(nameof(IsDeleteable), true);

    public static readonly StyledProperty<object> EditModeContentProperty =
        AvaloniaProperty.Register<EditableContainer, object>(nameof(EditModeContent));

    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<EditableContainer, object>(nameof(CommandParameter));

    public static readonly StyledProperty<ICommand> DeleteCommandProperty =
        AvaloniaProperty.Register<EditableContainer, ICommand>(nameof(DeleteCommand));

    public static readonly StyledProperty<ICommand> ApplyCommandProperty =
        AvaloniaProperty.Register<EditableContainer, ICommand>(nameof(ApplyCommand));

    public static readonly StyledProperty<ICommand> ToggleEditModeCommandProperty =
        AvaloniaProperty.Register<EditableContainer, ICommand>(nameof(ToggleEditModeCommand));

    public EditableContainer()
    {
        ToggleEditModeCommand = new RelayCommand(() =>
        {
            var block = (WidgetModel)DataContext!;
            block.IsEditMode = true;
            //ToDo: implement mechanism for custom behavior on edit mode toggle
        });

        ApplyCommand = new RelayCommand(() =>
        {
            var block = (WidgetModel)DataContext!;
            block.IsEditMode = false;
        });
    }

    public ICommand DeleteCommand
    {
        get => GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public ICommand ApplyCommand
    {
        get => GetValue(ApplyCommandProperty);
        set => SetValue(ApplyCommandProperty, value);
    }

    public bool IsEditMode
    {
        get => GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    public bool IsDeleteable
    {
        get => GetValue(IsDeleteableProperty);
        set => SetValue(IsDeleteableProperty, value);
    }

    public ICommand ToggleEditModeCommand
    {
        get => GetValue(ToggleEditModeCommandProperty);
        set => SetValue(ToggleEditModeCommandProperty, value);
    }

    public object EditModeContent
    {
        get => GetValue(EditModeContentProperty);
        set => SetValue(EditModeContentProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var beforeGrid = e.NameScope.Find<Grid>("before");
        var afterGrid = e.NameScope.Find<Grid>("after");
        var dragBtn = e.NameScope.Find<Panel>("dragBtn");

        dragBtn?.AddHandler(PointerPressedEvent, DoDrag);

        beforeGrid?.AddHandler(DragDrop.DropEvent, Drop);
        beforeGrid?.AddHandler(DragDrop.DragOverEvent, DragOver);

        afterGrid?.AddHandler(DragDrop.DropEvent, Drop);
        afterGrid?.AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    private async void DoDrag(object sender, PointerPressedEventArgs e)
    {
        var dragData = new DataObject();
        dragData.Set("block", DataContext);
        dragData.Set("index", GetModelIndex());
        var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
    }

    private int GetModelIndex()
    {
        var display = (Parent as ContentPresenter).Parent as ItemsControl;

        if (display is null)
        {
            throw new InvalidOperationException("Parent is not a BlockDisplay");
        }

        return display.Items.IndexOf((WidgetModel)DataContext);
    }

    private void DragOver(object sender, DragEventArgs e)
    {
        if (e.Source is Control c)
        {
            e.DragEffects &= DragDropEffects.Move;
        }
        else
        {
            e.DragEffects &= DragDropEffects.Copy;
        }
    }

    private void Drop(object sender, DragEventArgs e)
    {
        var index = (int)e.Data.Get("index");
        var block = (WidgetModel)e.Data.Get("block");
        var insertionIndex = GetModelIndex();

        if (sender is Grid g)
        {
            if (g.Name == "before")
            {
                insertionIndex--;
            }
            else if (g.Name == "after")
            {
                // insertionIndex++;
            }
        }

        var display = (Parent as ContentPresenter).Parent as ItemsControl;

        var items = display.Items.ToList();

        items.Insert(insertionIndex, block);
        items.RemoveAt(index);

        display.ItemsSource = items;
    }
}
