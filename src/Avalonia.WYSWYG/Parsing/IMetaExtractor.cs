namespace Avalonia.WYSWYG.Parsing;

public interface IMetaExtractor
{
    void ExtractToBlock(string meta, Block block);
}