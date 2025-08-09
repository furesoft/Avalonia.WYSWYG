using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM;

[ObservableObject]
public partial class EditorViewModel : INodeTemplatesHost, IEditor
{
    [ObservableProperty] private IDrawingNode _drawing;
    [ObservableProperty] private INodeFactory _factory;
    [ObservableProperty] private INodeSerializer _serializer;
    [ObservableProperty] private ObservableCollection<object> _templates;
}
