// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


using AdaptiveCards;
using AdaptiveCards.Rendering;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer.Helpers;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public static class AdaptiveColumnRenderer
{
    public static Control Render(AdaptiveColumn column, AdaptiveRenderContext context)
    {
        var uiContainer = new Grid();
        // uiContainer.Style = context.GetStyle("Adaptive.Column");

        var previousContextRtl = context.Rtl;
        var currentRtl = previousContextRtl;

        var updatedRtl = false;
        if (column.Rtl.HasValue)
        {
            currentRtl = column.Rtl;
            context.Rtl = currentRtl;
            updatedRtl = true;
        }

        if (currentRtl.HasValue)
            uiContainer.FlowDirection = currentRtl.Value ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        // Keep track of ContainerStyle.ForegroundColors before Container is rendered
        var parentRenderArgs = context.RenderArgs;
        // This is the renderArgs that will be passed down to the children
        var childRenderArgs = new AdaptiveRenderArgs(parentRenderArgs);

        var border = new Border();
        border.Child = uiContainer;

        var inheritsStyleFromParent = !column.Style.HasValue;
        var columnHasPadding = false;

        if (!inheritsStyleFromParent)
        {
            columnHasPadding =
                AdaptiveContainerRenderer.ApplyPadding(border, uiContainer, column, parentRenderArgs, context);

            // Apply background color
            var containerStyle = context.Config.ContainerStyles.GetContainerStyleConfig(column.Style);
            border.Background = context.GetColorBrush(containerStyle.BackgroundColor);

            childRenderArgs.ForegroundColors = containerStyle.ForegroundColors;
        }

        childRenderArgs.ParentStyle = inheritsStyleFromParent ? parentRenderArgs.ParentStyle : column.Style.Value;

        // If the column has no padding or has padding and doesn't bleed, then the children can bleed
        // to the side the column would have bled
        if (columnHasPadding) childRenderArgs.BleedDirection = BleedDirection.BleedAll;

        // If either this column or an ancestor had padding, then the children will have an ancestor with padding
        childRenderArgs.HasParentWithPadding = columnHasPadding || parentRenderArgs.HasParentWithPadding;
        context.RenderArgs = childRenderArgs;

        AdaptiveContainerRenderer.AddContainerElements(uiContainer, column.Items, context);

        RendererUtil.ApplyVerticalContentAlignment(border, column.VerticalContentAlignment);

        uiContainer.MinHeight = column.PixelMinHeight;

        // Revert context's value to that of outside the Column
        context.RenderArgs = parentRenderArgs;

        if (updatedRtl) context.Rtl = previousContextRtl;

        if (column.BackgroundImage != null)
        {
            uiContainer.SetBackgroundSource(column.BackgroundImage, context);
            if (column.Items.Count == 0)
                // if we have background source and no children we need to have at least padding to show background image 
                uiContainer.Children.Add(new Grid {Margin = new(context.Config.Spacing.Padding)});
        }

        return RendererUtil.ApplySelectAction(border, column, context);
    }
}