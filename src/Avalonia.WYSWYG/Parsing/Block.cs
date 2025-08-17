using System.Text.RegularExpressions;
using Splat;

namespace Avalonia.WYSWYG.Parsing;

public partial class Block
{
    public string Name { get; set; }
    public Dictionary<string, string> Properties { get; set; }

    public static Block Parse(string input)
    {
        var regex = BlockRegex();
        var match = regex.Match(input);

        if (!match.Success)
        {
            throw new ArgumentException($"Ungültiger Ausdruck: {input}");
        }

        var block = new Block
        {
            Name = match.Groups["blockname"].Value
        };
        block.Model = Locator.Current.GetService<WidgetStorage>()!.FindByName(block.Name);

        var metaGroup = match.Groups["meta"].Value;
        if (!string.IsNullOrEmpty(metaGroup))
        {
            if (block.Model != null)
            {
                block.Model.Extractor?.ExtractToBlock(metaGroup, block);
            }
            else
            {
                Locator.Current.GetService<PropertiesMetaExtractor>()!.ExtractToBlock(metaGroup, block);
            }
        }

        return block;
    }

    public WidgetModel? Model { get; set; }

    [GeneratedRegex(@"^/(?<blockname>\w+)(?:\[(?<meta>[^\]]*)\])?$")]
    private static partial Regex BlockRegex();
}