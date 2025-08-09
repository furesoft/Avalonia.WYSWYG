// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MarkedNet;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public class AdaptiveXamlMarkdownRenderer : MarkdownRenderer
{
    private static readonly string listMarker = "<ListMarker/>";

    public AdaptiveXamlMarkdownRenderer()
    {
    }

    public AdaptiveXamlMarkdownRenderer(Options options) : base(options)
    {
    }

    public override string Blockquote(string quote)
    {
        // not supported
        return string.Empty;
    }

    public override string Br()
    {
        return "<LineBreak/>\n";
    }

    public override string Code(string code, string lang, bool escaped)
    {
        // not supported
        return string.Empty;
    }

    public override string Codespan(string text)
    {
        return $"<Run FontFamily=\"Consolas\">{text}</Run>\n";
    }

    public override string Del(string text)
    {
        return $"<Run TextDecorations=\"Strikethrough\">{text}</Run>";
    }

    public override string Em(string text)
    {
        return $"<Italic>{text}</Italic>";
    }

    public override string Heading(string text, int level, string raw)
    {
        return $"{text}<LineBreak/>";
    }

    public override string Hr()
    {
        // not supported
        return string.Empty;
    }

    public override string Html(string html)
    {
        // not supported
        return string.Empty;
    }

    public override string Image(string href, string title, string text)
    {
        // not supported
        return string.Empty;
    }

    public override string Link(string href, string title, string text)
    {
        return $"{text} ({href})";
        //   return $"<Hyperlink Command=\"NavigationCommands.GoToPage\" CommandParameter=\"{href}\">{text}</Hyperlink>";
    }

    public override string List(string body, bool ordered, int start)
    {
        if (ordered)
        {
            int outputNum;
            int iMarker;
            for (outputNum = start, iMarker = body.IndexOf(listMarker);
                 iMarker > 0;
                 iMarker = body.IndexOf(listMarker), outputNum++)
                body = body.Remove(iMarker, listMarker.Length).Insert(iMarker, outputNum + ".");
        }
        else
        {
            body = body.Replace(listMarker, "•");
        }

        return $"{body}<LineBreak/>";
    }

    public override string ListItem(string text)
    {
        return $" {listMarker} {text}<LineBreak/>";
    }

    public override string Paragraph(string text)
    {
        return $"{text}<LineBreak/>";
    }

    public override string Strong(string text)
    {
        return $"<Bold>{text}</Bold>";
    }

    public override string Table(string header, string body)
    {
        // not supported
        return string.Empty;
    }

    public override string TableRow(string content)
    {
        // not supported
        return string.Empty;
    }

    public override string TableCell(string content, TableCellFlags flags)
    {
        // not supported
        return string.Empty;
    }

    public override string Postprocess(string text)
    {
        text = text.Replace("<LineBreak/><LineBreak/>", "<LineBreak/>");
        while (text.EndsWith("<LineBreak/>"))
            text = text.Substring(0, text.LastIndexOf("<LineBreak/>"));
        return base.Postprocess(text);
    }
}