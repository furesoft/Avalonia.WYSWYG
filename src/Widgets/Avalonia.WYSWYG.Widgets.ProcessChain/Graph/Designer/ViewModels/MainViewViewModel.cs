#nullable enable
using Avalonia.Platform.Storage;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Layout.ViewModels.Tools;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Services;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Services.Serializing;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;
using CommunityToolkit.Mvvm.Input;
using EditorViewModel = Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM.EditorViewModel;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    private readonly NodeFactory _nodeFactory;
    [ObservableProperty] private IDrawingNode? _drawing;

    [ObservableProperty] private EditorViewModel _editor;

    [ObservableProperty] private EmptyNode _selectedNode;
    [ObservableProperty] private string _text;

    [ObservableProperty] private ToolboxToolViewModel _toolboxToolViewModel;

    public MainViewViewModel()
    {
        _nodeFactory = new();

        _toolboxToolViewModel = new(_nodeFactory);

        _editor = new();

        _editor.Serializer = new NodeSerializer(typeof(ObservableCollection<>));
        _editor.Factory = _nodeFactory;
        _editor.Templates = new(_editor.Factory.CreateTemplates());

        _editor.Drawing = Drawing ?? _editor.Factory.CreateDrawing();

        _editor.Drawing.SetSerializer(_editor.Serializer);
    }

    public ObservableCollection<TreeViewItem> Items { get; set; } = new();


    private List<FilePickerFileType> GetOpenFileTypes()
    {
        return new() {StorageService.Project, StorageService.Json};
    }

    private static List<FilePickerFileType> GetSaveFileTypes()
    {
        return new() {StorageService.Json, StorageService.All};
    }


    [RelayCommand]
    private async Task Save()
    {
        var storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        var file = await storageProvider.SaveFilePickerAsync(new()
        {
            Title = "Save drawing",
            FileTypeChoices = GetSaveFileTypes(),
            SuggestedFileName = Path.GetFileNameWithoutExtension("graph"),
            DefaultExtension = "json",
            ShowOverwritePrompt = true
        });

        if (file is not null)
        {
            try
            {
                var json = Editor.Serializer.Serialize(Editor.Drawing);

                await using var stream = await file.OpenWriteAsync();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync((string?)json);
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
        }
    }
}
