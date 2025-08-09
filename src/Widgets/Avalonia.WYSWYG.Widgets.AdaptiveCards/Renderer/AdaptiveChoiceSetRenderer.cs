// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public static class AdaptiveChoiceSetRenderer
{
    public static Control Render(AdaptiveChoiceSetInput input, AdaptiveRenderContext context)
    {
        if (input.Style == AdaptiveChoiceInputStyle.Filtered)
        {
            var uiAutoComplete = new AutoCompleteBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Watermark = input.Placeholder
            };

            uiAutoComplete.ItemsSource = input.Choices.Select(choice => choice.Title);
            var inputValue = new AdaptiveChoiceSetInputValue(input, uiAutoComplete);
            context.InputValues.Add(input.Id, inputValue);
            return uiAutoComplete;
        }

        return RenderHelper(new(), new(), new(), input, context);
    }

    public static Control RenderHelper(Grid uiGrid, ComboBox uiComboBox, StackPanel uiChoices,
        AdaptiveChoiceSetInput input, AdaptiveRenderContext context)
    {
        uiComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
        uiComboBox.PlaceholderText = input.Placeholder;

        var chosen = input.Value?.Split(',').Select(p => p.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList() ??
                     new List<string>();

        uiGrid.RowDefinitions.Add(new() {Height = GridLength.Auto});
        uiGrid.RowDefinitions.Add(new() {Height = new(1, GridUnitType.Star)});

        //uiComboBox.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.ComboBox");
        uiComboBox.DataContext = input;

        uiChoices.DataContext = input;
        //uiChoices.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput");

        foreach (var choice in input.Choices)
            if (input.IsMultiSelect)
            {
                var uiCheckbox = new CheckBox
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                SetContent(uiCheckbox, choice.Title, input.Wrap);
                uiCheckbox.IsChecked = chosen.Contains(choice.Value);
                uiCheckbox.DataContext = choice;
                //uiCheckbox.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.CheckBox");
                uiChoices.Children.Add(uiCheckbox);
            }
            else
            {
                if (input.Style == AdaptiveChoiceInputStyle.Compact)
                {
                    var uiComboItem = new ComboBoxItem();
                    uiComboItem.HorizontalAlignment = HorizontalAlignment.Stretch;

                    //  uiComboItem.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.ComboBoxItem");
                    var content = SetContent(uiComboItem, choice.Title, input.Wrap);
                    // The content TextBlock is binded to the width of the comboBox container
                    if (input.Wrap && content != null)
                        content.SetValue(Layoutable.MaxWidthProperty, new Binding
                        {
                            Mode = BindingMode.OneWay,
                            Path = "ActualWidth",
                            Source = uiComboBox
                        });
                    //BindingOperations.SetBinding(content, TextBlock.MaxWidthProperty,
                    //    new Binding("ActualWidth") { Source = uiComboBox });
                    uiComboItem.DataContext = choice;

                    uiComboBox.Items.Add(uiComboItem);

                    // If multiple values are specified, no option is selected
                    if (chosen.Contains(choice.Value) && chosen.Count == 1) uiComboBox.SelectedItem = uiComboItem;
                }
                else
                {
                    var uiRadio = new RadioButton();
                    SetContent(uiRadio, choice.Title, input.Wrap);

                    // When isMultiSelect is false, only 1 specified value is accepted.
                    // Otherwise, don't set any option
                    if (chosen.Count == 1) uiRadio.IsChecked = chosen.Contains(choice.Value);
                    uiRadio.GroupName = input.Id;
                    uiRadio.DataContext = choice;
                    // uiRadio.Style = context.GetStyle("Adaptive.Input.AdaptiveChoiceSetInput.Radio");
                    uiChoices.Children.Add(uiRadio);
                }
            }

        AdaptiveChoiceSetInputValue inputValue = null;

        if (!input.IsMultiSelect && input.Style == AdaptiveChoiceInputStyle.Compact)
        {
            Grid.SetRow(uiComboBox, 1);
            uiGrid.Children.Add(uiComboBox);
            inputValue = new(input, uiComboBox);
        }
        else
        {
            Grid.SetRow(uiChoices, 1);
            uiGrid.Children.Add(uiChoices);
            inputValue = new(input, uiChoices, uiChoices.Children[0]);
        }

        if (input.IsRequired && string.IsNullOrEmpty(input.ErrorMessage))
            context.Warnings.Add(new((int) AdaptiveWarning.WarningStatusCode.NoErrorMessageForValidatedInput,
                "Inputs with validation should include an ErrorMessage"));

        context.InputValues.Add(input.Id, inputValue);

        return uiGrid;
    }

    public static TextBlock SetContent(ContentControl uiControl, string text, bool wrap)
    {
        if (wrap)
        {
            var wrappedTextBlock = new TextBlock {Text = text, TextWrapping = TextWrapping.Wrap};
            uiControl.Content = wrappedTextBlock;
            return wrappedTextBlock;
        }

        uiControl.Content = text;
        return null;
    }
}

public class AdaptiveChoiceSetInputValue : AdaptiveInputValueNonEmptyValidation
{
    private Control uIElement;

    public AdaptiveChoiceSetInputValue(AdaptiveChoiceSetInput inputElement, Control renderedElement) : base(
        inputElement, renderedElement)
    {
    }

    public AdaptiveChoiceSetInputValue(AdaptiveChoiceSetInput inputElement, Control renderedElement, Control uIElement)
        : base(inputElement, renderedElement, uIElement)
    {
    }

    public override string GetValue()
    {
        var choiceSet = InputElement as AdaptiveChoiceSetInput;

        if (choiceSet.Style == AdaptiveChoiceInputStyle.Filtered)
        {
            var uiAutoCompleteBox = RenderedInputElement as AutoCompleteBox;

            var value = choiceSet.Choices
                .Where(choice => string.Compare(uiAutoCompleteBox.Text, choice.Title, true) == 0)
                .Select(choice => choice.Value).FirstOrDefault();
            return value ?? string.Empty;
        }

        if (choiceSet.IsMultiSelect)
        {
            var uiChoices = RenderedInputElement as Panel;

            var values = string.Empty;
            foreach (var item in uiChoices.Children)
            {
                var checkBox = (CheckBox) item;
                var adaptiveChoice = checkBox.DataContext as AdaptiveChoice;
                if (checkBox.IsChecked == true)
                    values += (values == string.Empty ? "" : ",") + adaptiveChoice.Value;
            }

            return values;
        }

        var uiComboBox = RenderedInputElement as ComboBox;

        if (choiceSet.Style == AdaptiveChoiceInputStyle.Compact)
        {
            var item = uiComboBox.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                var adaptiveChoice = item.DataContext as AdaptiveChoice;
                return adaptiveChoice.Value;
            }

            return "";
        }

        {
            var uiChoices = RenderedInputElement as Panel;

            foreach (var item in uiChoices.Children)
            {
                var radioBox = (RadioButton) item;
                var adaptiveChoice = radioBox.DataContext as AdaptiveChoice;
                if (radioBox.IsChecked == true)
                    return adaptiveChoice.Value;
            }

            return "";
        }
    }

    public override void SetFocus()
    {
        // For expanded cases, the inputs are inserted into a panel,
        // so we focus on the first element in the panel
        if (RenderedInputElement is Panel)
        {
            var choicesContainer = RenderedInputElement as Panel;

            if (choicesContainer.Children.Count > 0) choicesContainer.Children[0].Focus();
        }
        else
        {
            RenderedInputElement.Focus();
        }
    }
}