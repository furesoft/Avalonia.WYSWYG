using System.Text.Json.Serialization;
using System.Windows.Input;
using Avalonia.Metadata;
using Avalonia.WYSWYG.Parsing;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

[assembly: XmlnsDefinition("http://furesoft.de/schemas/wyswyg", "Avalonia.WYSWYG")]
[assembly: XmlnsDefinition("http://furesoft.de/schemas/wyswyg", "Avalonia.WYSWYG.Controls")]

namespace Avalonia.WYSWYG;

public sealed partial class WidgetModel : ObservableObject
{
    [ObservableProperty] private MaterialIconKind _iconKind;
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _displayName;
    [ObservableProperty] private string _category;
    [ObservableProperty] private string? _command;
    public IMetaExtractor? Extractor { get; set; }

    public WidgetModel()
    {
        IconKind = MaterialIconKind.ArrowRight;
        ID = Guid.NewGuid();
    }

    [JsonIgnore] public WidgetTemplate EditViewTemplate { get; set; }
    [JsonIgnore] public WidgetTemplate Template { get; set; }

    public ObservableDictionary<string, object> Properties { get; set; } = new();

    public Guid ID { get; set; }
    public ICommand OnAddCommand { get; set; }

    public T? GetProperty<T>(string name)
    {
        if (Properties.TryGetValue(name, out var value))
        {
            return (T)value;
        }

        return default;
    }

    public void SetProperty(string name, object value)
    {
        if (!Properties.TryAdd(name, value))
        {
            Properties[name] = value;
        }
    }

    public override string ToString()
    {
        return _command;
    }
}
