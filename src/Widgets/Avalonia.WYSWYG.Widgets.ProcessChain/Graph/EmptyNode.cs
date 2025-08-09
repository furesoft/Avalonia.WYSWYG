using System.ComponentModel;
using System.Runtime.Serialization;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.ViewModels;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.NodeViews;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph;

[DataContract(IsReference = true)]
public abstract partial class EmptyNode : ViewModelBase, ICustomTypeDescriptor
{
    private string _description;
    private string _label;
    private bool _showDescription;

    protected EmptyNode(string label)
    {
        Label = label;
        ID = Guid.NewGuid();
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Browsable(false)]
    [JsonIgnore]
    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    [Browsable(false)]
    [DataMember(EmitDefaultValue = false)]
    public Guid ID { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Browsable(false)]
    [JsonIgnore]
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool ShowDescription
    {
        get => _showDescription;
        set => SetProperty(ref _showDescription, value);
    }

    public Control GetView(ref double width, ref double height)
    {
        Control nodeView = new DefaultNodeView();

        var nodeViewAttribute = GetAttribute<NodeViewAttribute>();

        if (nodeViewAttribute != null)
        {
            nodeView = (Control)Activator.CreateInstance(nodeViewAttribute.Type);

            if (nodeView!.MinHeight > height)
            {
                height = nodeView.MinHeight;
            }

            if (nodeView.MinWidth > width)
            {
                width = nodeView.MinWidth;
            }

            nodeView.Tag = nodeViewAttribute.Parameter;
        }

        return nodeView;
    }

    public virtual void OnInit()
    {
    }

    public T GetAttribute<T>()
        where T : Attribute
    {
        return TypeDescriptor.GetAttributes(this)
            .OfType<T>().FirstOrDefault();
    }
}
