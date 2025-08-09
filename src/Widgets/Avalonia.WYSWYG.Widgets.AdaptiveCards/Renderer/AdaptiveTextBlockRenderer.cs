// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;
using AdaptiveCards;
using AdaptiveCards.Rendering;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer.Helpers;
using Microsoft.MarkedNet;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public static class AdaptiveTextBlockRenderer
{
    public static Control Render(AdaptiveTextBlock textBlock, AdaptiveRenderContext context)
    {
        if (string.IsNullOrEmpty(textBlock.Text)) return null;

        var uiTextBlock = CreateControl(textBlock, context);

        uiTextBlock.SetColor(textBlock.Color, textBlock.IsSubtle, context);

        if (textBlock.MaxWidth > 0) uiTextBlock.MaxWidth = textBlock.MaxWidth;

        if (textBlock.MaxLines > 0) uiTextBlock.MaxLines = textBlock.MaxLines;

        return uiTextBlock;
    }

    private static TextBlock CreateControl(AdaptiveTextBlock textBlock, AdaptiveRenderContext context)
    {
        var marked = new Marked();
        marked.Options.Renderer = new AdaptiveXamlMarkdownRenderer();
        marked.Options.Mangle = false;
        marked.Options.Sanitize = true;

        var text = textBlock.Text;
        text = RendererUtilities.ApplyTextFunctions(text, context.Lang);
        text = marked.Parse(text);
        text = RendererUtilities.HandleHtmlSpaces(text);

        var xaml = $"<TextBlock  xmlns=\"https://github.com/avaloniaui\">{text}</TextBlock>";
        // string xaml = $"<TextBlock >{text}</TextBlock>";
        var uiTextBlock = AvaloniaRuntimeXamlLoader.Parse<TextBlock>(xaml);
        // uiTextBlock.Style = context.GetStyle($"Adaptive.{textBlock.Type}");

        uiTextBlock.TextWrapping = TextWrapping.NoWrap;
        uiTextBlock.FontFamily =
            new(RendererUtil.GetFontFamilyFromList(context.Config.GetFontFamily(textBlock.FontType)));
        uiTextBlock.TextTrimming = TextTrimming.CharacterEllipsis;

        if (textBlock.Style == AdaptiveTextBlockStyle.Heading)
        {
            uiTextBlock.FontWeight =
                (FontWeight) context.Config.GetFontWeight(textBlock.FontType, AdaptiveTextWeight.Bolder);
            uiTextBlock.FontSize = context.Config.GetFontSize(textBlock.FontType, textBlock.Size switch
            {
                AdaptiveTextSize.Small => AdaptiveTextSize.Medium,
                AdaptiveTextSize.Medium => AdaptiveTextSize.Large,
                AdaptiveTextSize.Default => AdaptiveTextSize.Large,
                AdaptiveTextSize.Large => AdaptiveTextSize.ExtraLarge,
                AdaptiveTextSize.ExtraLarge => AdaptiveTextSize.ExtraLarge
            });
        }
        else
        {
            uiTextBlock.FontWeight = (FontWeight) context.Config.GetFontWeight(textBlock.FontType, textBlock.Weight);
            uiTextBlock.FontSize = context.Config.GetFontSize(textBlock.FontType, textBlock.Size);
        }

        if (textBlock.Italic) uiTextBlock.FontStyle = FontStyle.Italic;

        if (textBlock.Strikethrough) uiTextBlock.TextDecorations = TextDecorations.Strikethrough;

        if (textBlock.HorizontalAlignment != AdaptiveHorizontalAlignment.Left)
        {
            TextAlignment alignment;
            if (Enum.TryParse(textBlock.HorizontalAlignment.ToString(), out alignment))
                uiTextBlock.TextAlignment = alignment;
        }

        if (textBlock.Wrap)
            uiTextBlock.TextWrapping = TextWrapping.Wrap;

        return uiTextBlock;
    }

    private class ALink
    {
        public int Start { get; set; }

        public string Original { get; set; }
        public string Text { get; set; }

        public string Url { get; set; }
    }

    private class MultiplyConverter : IValueConverter
    {
        private readonly int multiplier;

        public MultiplyConverter(int multiplier)
        {
            this.multiplier = multiplier;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double) value * multiplier;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double) value * multiplier;
        }
    }
}