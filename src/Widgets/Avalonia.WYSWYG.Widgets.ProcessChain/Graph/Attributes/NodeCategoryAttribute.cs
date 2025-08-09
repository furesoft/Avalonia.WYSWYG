namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class NodeCategoryAttribute : Attribute
{
    public NodeCategoryAttribute(string category)
    {
        Category = category;
    }

    public NodeCategoryAttribute()
    {
    }

    public string Category { get; set; }
}
