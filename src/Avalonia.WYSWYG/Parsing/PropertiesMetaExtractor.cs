namespace Avalonia.WYSWYG.Parsing;

public class PropertiesMetaExtractor : IMetaExtractor
{
    public void ExtractToBlock(string meta, Block block)
    {
        var propPairs = meta.Split(',');

        foreach (var pair in propPairs)
        {
            if (string.IsNullOrWhiteSpace(pair)) continue;

            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                block.Properties[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }
    }
}