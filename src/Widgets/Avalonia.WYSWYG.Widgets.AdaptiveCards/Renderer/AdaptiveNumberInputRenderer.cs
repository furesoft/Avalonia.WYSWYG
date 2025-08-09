// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer.Helpers;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public static class AdaptiveNumberInputRenderer
{
    public static Control Render(AdaptiveNumberInput input, AdaptiveRenderContext context)
    {
        var uiInput = new NumericUpDown
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        if (!double.IsNaN(input.Value)) uiInput.Value = (decimal) input.Value;

        // textBox.Style = context.GetStyle($"Adaptive.Input.Text.Number");
        uiInput.SetContext(input);

        if (!double.IsNaN(input.Min))
            uiInput.Minimum = (decimal) input.Min;
        if (!double.IsNaN(input.Max))
            uiInput.Maximum = (decimal) input.Max;

        if ((!double.IsNaN(input.Max) || !double.IsNaN(input.Min) || input.IsRequired)
            && string.IsNullOrEmpty(input.ErrorMessage))
            context.Warnings.Add(new((int) AdaptiveWarning.WarningStatusCode.NoErrorMessageForValidatedInput,
                "Inputs with validation should include an ErrorMessage"));

        context.InputValues.Add(input.Id, new AdaptiveNumberInputValue(input, uiInput));

        return uiInput;
    }
}

public class AdaptiveNumberInputValue : AdaptiveTextBoxInputValue
{
    public AdaptiveNumberInputValue(AdaptiveNumberInput inputElement, Control renderedElement) : base(inputElement,
        renderedElement)
    {
    }

    public override string GetValue()
    {
        return (RenderedInputElement as NumericUpDown).Value?.ToString();
    }

    public override bool Validate()
    {
        var isValid = base.Validate();

        var numberInput = InputElement as AdaptiveNumberInput;
        var inputValue = 0.0;

        if (isValid && double.TryParse(GetValue(), out inputValue))
        {
            bool isMinValid = true, isMaxValid = true;
            if (!double.IsNaN(numberInput.Min)) isMinValid = inputValue >= numberInput.Min;

            if (!double.IsNaN(numberInput.Max)) isMaxValid = inputValue <= numberInput.Max;

            isValid = isValid && isMinValid && isMaxValid;
        }
        else
        {
            // if the input is not required and the string is empty, then proceed
            // This is a fail safe as non xceed controls are rendered with a Text
            if (!(!numberInput.IsRequired && string.IsNullOrEmpty(GetValue()))) isValid = false;
        }

        return isValid;
    }
}