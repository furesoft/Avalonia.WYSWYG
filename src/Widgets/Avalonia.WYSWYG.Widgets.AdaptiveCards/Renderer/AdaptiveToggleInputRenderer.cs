// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer.Helpers;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public static class AdaptiveToggleInputRenderer
{
    public static Control Render(AdaptiveToggleInput input, AdaptiveRenderContext context)
    {
        var uiToggle = new CheckBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        AdaptiveChoiceSetRenderer.SetContent(uiToggle, input.Title, input.Wrap);
        uiToggle.Foreground =
            context.GetColorBrush(context.Config.ContainerStyles.Default.ForegroundColors.Default.Default);
        uiToggle.SetState(input.Value == (input.ValueOn ?? "true"));
        // uiToggle.Style = context.GetStyle($"Adaptive.Input.Toggle");
        uiToggle.SetContext(input);

        if (input.IsRequired && string.IsNullOrEmpty(input.ErrorMessage))
            context.Warnings.Add(new((int) AdaptiveWarning.WarningStatusCode.NoErrorMessageForValidatedInput,
                "Inputs with validation should include an ErrorMessage"));

        context.InputValues.Add(input.Id, new AdaptiveToggleInputValue(input, uiToggle));

        return uiToggle;
    }
}

public class AdaptiveToggleInputValue : AdaptiveInputValue
{
    public AdaptiveToggleInputValue(AdaptiveToggleInput inputElement, Control renderedElement) : base(inputElement,
        renderedElement)
    {
    }

    public override string GetValue()
    {
        var uiToggle = RenderedInputElement as CheckBox;
        var toggleInput = InputElement as AdaptiveToggleInput;

        return uiToggle.GetState() == true ? toggleInput.ValueOn ?? "true" : toggleInput.ValueOff ?? "false";
    }

    public override void SetFocus()
    {
        RenderedInputElement.Focus();
    }

    public override bool Validate()
    {
        var isValid = true;

        if (InputElement.IsRequired)
        {
            var toggleInput = InputElement as AdaptiveToggleInput;
            isValid = GetValue() == toggleInput.ValueOn;
        }

        return isValid;
    }
}