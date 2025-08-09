namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

public interface INodeSerializer
{
    string Serialize<T>(T value);
    T Deserialize<T>(string text);
}
