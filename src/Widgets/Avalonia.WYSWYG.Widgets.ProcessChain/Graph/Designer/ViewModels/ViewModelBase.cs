using System.ComponentModel;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.ViewModels;

public class ViewModelBase : ObservableObject
{
    [Browsable(false)] public IDrawingNode Drawing { get; set; }

    public event Action OnRequestClose;

    [JsonIgnore, Browsable(false)]
    public bool IsLoaded { get; set; }

    public void Load()
    {
        if (!IsLoaded)
        {
            IsLoaded = true;

            OnLoad();
        }
    }

    protected virtual void OnLoad()
    {
    }

    protected void RequestClose()
    {
        OnRequestClose?.Invoke();
    }
}
