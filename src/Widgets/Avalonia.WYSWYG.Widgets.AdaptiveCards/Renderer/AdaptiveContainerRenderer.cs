// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using AdaptiveCards.Rendering;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer.Helpers;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public static class AdaptiveContainerRenderer
{
    public static Control Render(AdaptiveContainer container, AdaptiveRenderContext context)
    {
        var uiContainer = new Grid();
        // uiContainer.Style = context.GetStyle("Adaptive.Container");

        var previousContextRtl = context.Rtl;
        var currentRtl = previousContextRtl;

        var updatedRtl = false;
        if (container.Rtl.HasValue)
        {
            currentRtl = container.Rtl;
            context.Rtl = currentRtl;
            updatedRtl = true;
        }

        if (currentRtl.HasValue)
            uiContainer.FlowDirection = currentRtl.Value ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        // Keep track of ContainerStyle.ForegroundColors before Container is rendered
        var parentRenderArgs = context.RenderArgs;
        // This is the renderArgs that will be passed down to the children
        var childRenderArgs = new AdaptiveRenderArgs(parentRenderArgs);

        var uiOuterContainer = new Grid();

        uiOuterContainer.Children.Add(uiContainer);
        var border = new Border();
        border.Child = uiOuterContainer;

        RendererUtil.ApplyVerticalContentAlignment(border, container.VerticalContentAlignment);
        uiContainer.MinHeight = container.PixelMinHeight;

        var inheritsStyleFromParent = !container.Style.HasValue;
        var hasPadding = false;
        if (!inheritsStyleFromParent)
        {
            hasPadding = ApplyPadding(border, uiOuterContainer, container, parentRenderArgs, context);

            // Apply background color
            var containerStyle = context.Config.ContainerStyles.GetContainerStyleConfig(container.Style);
            border.Background = context.GetColorBrush(containerStyle.BackgroundColor);

            childRenderArgs.ForegroundColors = containerStyle.ForegroundColors;
        }

        RendererUtil.ApplyVerticalContentAlignment(uiContainer, container.VerticalContentAlignment);

        if (hasPadding) childRenderArgs.BleedDirection = BleedDirection.BleedAll;

        // Modify context outer parent style so padding necessity can be determined
        childRenderArgs.ParentStyle = inheritsStyleFromParent ? parentRenderArgs.ParentStyle : container.Style.Value;
        childRenderArgs.HasParentWithPadding = hasPadding || parentRenderArgs.HasParentWithPadding;
        context.RenderArgs = childRenderArgs;

        AddContainerElements(uiContainer, container.Items, context);

        // Revert context's value to that of outside the Container
        context.RenderArgs = parentRenderArgs;

        if (updatedRtl) context.Rtl = previousContextRtl;
        if (container.BackgroundImage != null)
        {
            uiContainer.SetBackgroundSource(container.BackgroundImage, context);
            if (container.Items.Count == 0)
                // if we have background source and no children we need to have a 10x10 min size to show background image (this is to align with
                // with html behavior, this is not documented anywhere.
                uiContainer.Children.Add(new Grid {Margin = new(10, 10, 10, 10)});
        }

        return RendererUtil.ApplySelectAction(border, container, context);
    }

    public static void AddContainerElements(Grid uiContainer, IList<AdaptiveElement> elements,
        AdaptiveRenderContext context, Func<Control, Control> processContainerElement = null)
    {
        // Keeping track of the index so we don't have to call IndexOf function on every iteration
        var index = 0;
        foreach (var cardElement in elements)
        {
            if (index != 0)
                // Only the first element can bleed to the top
                context.RenderArgs.BleedDirection &= ~BleedDirection.BleedUp;

            if (index != elements.Count - 1)
                // Only the last element can bleed to the bottom
                context.RenderArgs.BleedDirection &= ~BleedDirection.BleedDown;

            index++;

            // each element has a row
            var rendereableElement = context.GetRendereableElement(cardElement);

            // if there's an element that can be rendered, then render it, otherwise, skip
            if (rendereableElement != null)
            {
                var uiElement = context.Render(rendereableElement);
                if (uiElement != null)
                {
                    uiElement = processContainerElement?.Invoke(uiElement) ?? uiElement;

                    TagContent tag = null;
                    Grid separator = null;
                    if (cardElement.Separator && uiContainer.Children.Count > 0)
                        separator = AddSeparator(context, cardElement, uiContainer);
                    else if (uiContainer.Children.Count > 0) separator = AddSpacing(context, cardElement, uiContainer);

                    tag = new(separator, uiContainer);

                    uiElement.Tag = tag;

                    // Sets the minHeight property for Container and ColumnSet
                    if (cardElement.Type == "Container" || cardElement.Type == "ColumnSet")
                    {
                        var collectionElement = (AdaptiveCollectionElement) cardElement;
                        uiElement.MinHeight = collectionElement.PixelMinHeight;
                    }

                    // If the rendered element is an input element, then add it to the inputs list
                    if (rendereableElement is AdaptiveInput)
                        context.AddInputToCard(context.RenderArgs.ContainerCardId, rendereableElement.Id);

                    var rowDefinitionIndex = uiContainer.RowDefinitions.Count;
                    RowDefinition rowDefinition = null;
                    if (cardElement.Height != AdaptiveHeight.Stretch)
                    {
                        rowDefinition = new() {Height = GridLength.Auto};

                        uiContainer.RowDefinitions.Add(rowDefinition);

                        var enclosingElement = EncloseElementInPanel(context, rendereableElement, uiElement);

                        Grid.SetRow(enclosingElement, rowDefinitionIndex);
                        uiContainer.Children.Add(enclosingElement);

                        // Row definition is stored in the tag for containers and elements that stretch
                        // so when the elements are shown, the row can have it's original definition,
                        // while when the element is hidden, the extra space is not reserved in the layout
                        tag.RowDefinition = rowDefinition;
                        tag.ViewIndex = rowDefinitionIndex;

                        enclosingElement.Tag = tag;

                        if (!string.IsNullOrEmpty(cardElement.Id))
                            // All elements are added to the dictionary when rendered but elements
                            // with labels or errorMessages have to be enclosed so that has to be added instead
                            context.RenderedElementsWithId[cardElement.Id] = enclosingElement;

                        RendererUtil.SetVisibility(enclosingElement, cardElement.IsVisible, tag);
                    }
                    else
                    {
                        rowDefinition = new() {Height = new(1, GridUnitType.Star)};
                        uiContainer.RowDefinitions.Add(rowDefinition);

                        // Row definition is stored in the tag for containers and elements that stretch
                        // so when the elements are shown, the row can have it's original definition,
                        // while when the element is hidden, the extra space is not reserved in the layout
                        tag.RowDefinition = rowDefinition;
                        tag.ViewIndex = rowDefinitionIndex;

                        if (cardElement.Type == "Container")
                        {
                            Grid.SetRow(uiElement, rowDefinitionIndex);
                            uiContainer.Children.Add(uiElement);
                            RendererUtil.SetVisibility(uiElement, cardElement.IsVisible, tag);
                        }
                        else
                        {
                            var panel = new StackPanel();

                            if (!string.IsNullOrEmpty(cardElement.Id))
                                // All elements are added to the dictionary when rendered but elements
                                // with height stretch are enclosed in a panel that must be added instead
                                context.RenderedElementsWithId.Add(cardElement.Id, panel);

                            if (rendereableElement is AdaptiveInput)
                                panel.Children.Add(EncloseInputForVisualCue(context,
                                    rendereableElement as AdaptiveInput, uiElement));
                            else
                                panel.Children.Add(uiElement);
                            panel.Tag = tag;

                            Grid.SetRow(panel, rowDefinitionIndex);
                            uiContainer.Children.Add(panel);
                            RendererUtil.SetVisibility(panel, cardElement.IsVisible, tag);
                        }
                    }
                }
            }
        }

        context.ResetSeparatorVisibilityInsideContainer(uiContainer);
    }

    /// <summary>
    ///     Adds spacing as a grid element to the container
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="element">Element to render spacing for</param>
    /// <param name="uiContainer">Container of rendered elements</param>
    /// <returns>Spacing grid</returns>
    public static Grid AddSpacing(AdaptiveRenderContext context, AdaptiveElement element, Panel uiContainer)
    {
        return AddSpacing(context, element.Spacing, uiContainer);
    }

    public static Grid AddSpacing(AdaptiveRenderContext context, AdaptiveSpacing spacing, Panel uiContainer)
    {
        if (spacing == AdaptiveSpacing.None) return null;

        var uiSpa = new Grid();
        // uiSpa.Style = context.GetStyle($"Adaptive.Spacing");

        var pixelSpacing = context.Config.GetSpacing(spacing);
        uiSpa.SetHeight(pixelSpacing);

        if (uiContainer is Grid)
        {
            var uiContainerGrid = uiContainer as Grid;
            uiContainerGrid.RowDefinitions.Add(new() {Height = GridLength.Auto});
            Grid.SetRow(uiSpa, uiContainerGrid.RowDefinitions.Count - 1);
        }

        uiContainer.Children.Add(uiSpa);

        return uiSpa;
    }

    public static Grid AddSeparator(AdaptiveRenderContext context, AdaptiveElement element, Grid uiContainer)
    {
        if (element.Spacing == AdaptiveSpacing.None && !element.Separator) return null;

        var uiSep = new Grid();
        // uiSep.Style = context.GetStyle($"Adaptive.Separator");
        var spacing = context.Config.GetSpacing(element.Spacing);

        var sepStyle = context.Config.Separator;

        var margin = (spacing - sepStyle.LineThickness) / 2;
        uiSep.Margin = new(0, margin, 0, margin);
        uiSep.SetHeight(sepStyle.LineThickness);
        if (!string.IsNullOrWhiteSpace(sepStyle.LineColor)) uiSep.SetBackgroundColor(sepStyle.LineColor, context);
        uiContainer.RowDefinitions.Add(new() {Height = GridLength.Auto});
        Grid.SetRow(uiSep, uiContainer.RowDefinitions.Count - 1);
        uiContainer.Children.Add(uiSep);

        return uiSep;
    }

    private static Thickness GetBleedMargin(AdaptiveRenderArgs parentRenderArgs, int padding)
    {
        return new(
            (parentRenderArgs.BleedDirection & BleedDirection.BleedLeft) != BleedDirection.BleedNone ? padding : 0,
            (parentRenderArgs.BleedDirection & BleedDirection.BleedUp) != BleedDirection.BleedNone ? padding : 0,
            (parentRenderArgs.BleedDirection & BleedDirection.BleedRight) != BleedDirection.BleedNone ? padding : 0,
            (parentRenderArgs.BleedDirection & BleedDirection.BleedDown) != BleedDirection.BleedNone ? padding : 0);
    }

    // For applying bleeding, we must know if the element has padding, so both properties are applied in the same method
    public static bool ApplyPadding(Border border, Grid uiElement, AdaptiveCollectionElement element,
        AdaptiveRenderArgs parentRenderArgs, AdaptiveRenderContext context)
    {
        var canApplyPadding = false;

        // AdaptiveColumn inherits from AdaptiveContainer so only one check is required for both
        if (element is AdaptiveContainer container)
            canApplyPadding = container.BackgroundImage != null ||
                              (container.Style.HasValue && container.Style != parentRenderArgs.ParentStyle);
        else if (element is AdaptiveColumnSet columnSet)
            canApplyPadding = columnSet.Style.HasValue && columnSet.Style != parentRenderArgs.ParentStyle;

        var padding = context.Config.Spacing.Padding;

        if (canApplyPadding)
        {
            uiElement.Margin = new(padding);

            if (element.Bleed) border.Margin = GetBleedMargin(parentRenderArgs, -padding);
        }

        return canApplyPadding;
    }

    /// <summary>
    ///     Encloses the element in a panel to ease the showing/hiding of elements
    /// </summary>
    /// <param name="renderedElement">Control</param>
    /// <returns>Panel that encloses the element</returns>
    public static Control EncloseElementInPanel(AdaptiveRenderContext context, AdaptiveTypedElement element,
        Control renderedElement)
    {
        var enclosingElement = renderedElement;

        if (element is AdaptiveInput)
        {
            var inputElement = element as AdaptiveInput;
            enclosingElement = EncloseInputForVisualCue(context, inputElement, renderedElement);

            var elementForAccessibility = renderedElement;

            if (inputElement is AdaptiveChoiceSetInput)
                if (renderedElement is Grid)
                {
                    var inputContainer = renderedElement as Grid;
                    // ChoiceSet inputs render by returning a Grid. The grid may contain a combobox or a panel that contains the options
                    var choiceSet = inputContainer.Children[0];

                    if (choiceSet is ComboBox)
                        elementForAccessibility = choiceSet;
                    else if (choiceSet is Panel)
                        // If it's a choice set, then use the first element as element
                        elementForAccessibility = (choiceSet as Panel).Children[0] ?? renderedElement;
                }

            // AutomationProperties.SetIsRequiredForForm(GetVisualElementForAccessibility(context, inputElement) ?? elementForAccessibility, inputElement.IsRequired);

            if (!string.IsNullOrEmpty(inputElement.Label) || !string.IsNullOrEmpty(inputElement.ErrorMessage))
            {
                var tag = renderedElement.Tag as TagContent;

                var panel = new StackPanel();

                if (!string.IsNullOrEmpty(inputElement.Label))
                {
                    panel.Children.Add(RenderLabel(context, inputElement, elementForAccessibility));
                    AddSpacing(context, context.Config.Inputs.Label.InputSpacing, panel);
                }
                else if (inputElement.IsRequired)
                {
                    context.Warnings.Add(new((int) AdaptiveWarning.WarningStatusCode.EmptyLabelInRequiredInput,
                        "Input is required but there's no label for required hint rendering"));
                }

                panel.Children.Add(enclosingElement);
                enclosingElement = panel;
                tag.EnclosingElement = panel;

                if (!string.IsNullOrEmpty(inputElement.ErrorMessage))
                {
                    var errorMessageSpacing = AddSpacing(context, context.Config.Inputs.ErrorMessage.Spacing, panel);

                    var renderedErrorMessage = RenderErrorMessage(context, inputElement);
                    renderedErrorMessage.Tag = new TagContent(errorMessageSpacing, null);

                    if (context.Config.SupportsInteractivity)
                        context.InputValues[inputElement.Id].ErrorMessage = renderedErrorMessage;

                    panel.Children.Add(renderedErrorMessage);
                    tag.ErrorMessage = renderedErrorMessage;

                    RendererUtil.SetVisibility(renderedErrorMessage, false, renderedErrorMessage.Tag as TagContent);
                }
            }
        }

        return enclosingElement;
    }

    /// <summary>
    ///     Encloses the element in a panel to ease the showing/hiding of elements
    /// </summary>
    /// <param name="renderedElement">Control</param>
    /// <returns>Border that encloses the element or the element if not needed</returns>
    public static Control EncloseInputForVisualCue(AdaptiveRenderContext context, AdaptiveInput element,
        Control renderedElement)
    {
        var enclosingElement = renderedElement;

        if (!(element is AdaptiveChoiceSetInput) && !(element is AdaptiveToggleInput))
        {
            var inputValue = context.InputValues[element.Id] as AdaptiveTextBoxInputValue;

            if (inputValue != null)
            {
                var visualCue = new Border();
                visualCue.Child = renderedElement;
                visualCue.BorderBrush =
                    context.GetColorBrush(context.GetForegroundColors(AdaptiveTextColor.Attention).Default);

                inputValue.VisualErrorCue = visualCue;
                enclosingElement = visualCue;
            }
        }

        return enclosingElement;
    }

    /// <summary>
    ///     Renders the label for an input element
    /// </summary>
    /// <param name="context">AdaptiveRenderContext</param>
    /// <param name="input">AdaptiveInput</param>
    /// <returns>The rendered label</returns>
    public static Control RenderLabel(AdaptiveRenderContext context, AdaptiveInput input, Control renderedInput)
    {
        var uiTextBlock = new TextBlock {TextWrapping = TextWrapping.Wrap};

        InputLabelConfig labelConfig = null;
        if (input.IsRequired)
            labelConfig = context.Config.Inputs.Label.RequiredInputs;
        else
            labelConfig = context.Config.Inputs.Label.OptionalInputs;

        Inline labelTextInline = new Run(input.Label);
        labelTextInline.SetColor(labelConfig.Color, labelConfig.IsSubtle, context);
        uiTextBlock.Inlines.Add(labelTextInline);

        if (input.IsRequired)
        {
            var hintToRender = " *";
            if (string.IsNullOrWhiteSpace(labelConfig.Suffix)) hintToRender = labelConfig.Suffix;

            Inline requiredHintInline = new Run(hintToRender);
            requiredHintInline.SetColor(AdaptiveTextColor.Attention, labelConfig.IsSubtle, context);
            uiTextBlock.Inlines.Add(requiredHintInline);
        }

        uiTextBlock.FontWeight =
            (FontWeight) context.Config.GetFontWeight(AdaptiveFontType.Default, labelConfig.Weight);
        uiTextBlock.FontSize = context.Config.GetFontSize(AdaptiveFontType.Default, labelConfig.Size);

        // For Input.Text we render inline actions inside of a Grid, so we set the property
        //AutomationProperties.SetLabeledBy(GetVisualElementForAccessibility(context, input) ?? renderedInput, uiTextBlock);

        return uiTextBlock;
    }

    /// <summary>
    ///     Renders the error message for an input element
    /// </summary>
    /// <param name="context">AdaptiveRenderContext</param>
    /// <param name="input">AdaptiveInput</param>
    /// <returns>The rendered error message</returns>
    public static TextBlock RenderErrorMessage(AdaptiveRenderContext context, AdaptiveInput input)
    {
        var uiTextBlock = new TextBlock
        {
            Text = input.ErrorMessage,
            TextWrapping = TextWrapping.Wrap,
            IsVisible = false
        };

        // By default the color is set to attention
        uiTextBlock.SetColor(AdaptiveTextColor.Attention, false /* isSubtle */, context);

        // Then we honour size and weight from hostconfig
        uiTextBlock.FontWeight =
            (FontWeight) context.Config.GetFontWeight(AdaptiveFontType.Default,
                context.Config.Inputs.ErrorMessage.Weight);
        uiTextBlock.FontSize =
            context.Config.GetFontSize(AdaptiveFontType.Default, context.Config.Inputs.ErrorMessage.Size);

        return uiTextBlock;
    }

    public static Control GetVisualElementForAccessibility(AdaptiveRenderContext context, AdaptiveInput input)
    {
        return context.InputValues[input.Id]?.VisualElementForAccessibility;
    }
}