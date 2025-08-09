using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Avalonia.WYSWYG.Controls;

[TemplatePart("PART_Items", typeof(SelectingItemsControl))]
[TemplatePart("PART_Popup", typeof(ImagePopupButton))]
public class AddWidgetButton : TemplatedControl
{
    public static readonly StyledProperty<bool> IsPopupOpenProperty =
        AvaloniaProperty.Register<AddWidgetButton, bool>(nameof(IsPopupOpen));


    public static readonly StyledProperty<WidgetDisplay> DisplayProperty =
        AvaloniaProperty.Register<AddWidgetButton, WidgetDisplay>(nameof(Display));

    private ImagePopupButton _popupButton;

    public bool IsPopupOpen
    {
        get => GetValue(IsPopupOpenProperty);
        set => SetValue(IsPopupOpenProperty, value);
    }

    public WidgetDisplay Display
    {
        get => GetValue(DisplayProperty);
        set => SetValue(DisplayProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var lb = (SelectingItemsControl)e.NameScope.Find("PART_Items");
        _popupButton = (ImagePopupButton)e.NameScope.Find("PART_Popup");

        lb!.SelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        var lbs = (SelectingItemsControl)sender;
        var model = lbs.SelectedItem as WidgetModel;

        if (model is null)
        {
            return;
        }

        var blockStorage = ContainerLocator.Current.Resolve<WidgetStorage>();
        var block = blockStorage.Instanciate(model.ID);

        block.IsEditMode = true;
        Display.Items.Add(block);
        block?.OnAddCommand?.Execute(block);

        _popupButton.Toggle();

        lbs.SelectedItem = null;
    }
}
