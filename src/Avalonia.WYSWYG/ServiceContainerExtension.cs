using Avalonia.Markup.Xaml;
using Splat;

namespace Avalonia.WYSWYG;

public class ServiceContainerExtension(Type type) : MarkupExtension
{
    public Type Type { get; set; } = type;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Locator.Current.GetService(Type);
    }
}
