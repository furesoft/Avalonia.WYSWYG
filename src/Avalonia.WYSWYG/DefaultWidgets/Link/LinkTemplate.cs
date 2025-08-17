using Avalonia.Controls;
using Avalonia.Data;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.WYSWYG.WidgetTemplates.Link;

public class LinkTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        return new Link
        {
            [!Link.URLProperty] = new Binding("Properties[URL]", BindingMode.TwoWay),
            [!Link.TitleProperty] = new Binding("Properties[Label]", BindingMode.TwoWay),
            Command = new RelayCommand(() => Link_Clicked(model.GetProperty<string>("URL")))
        };
    }

    private void Link_Clicked(string? url)
    {
        var uri = new Uri(url);

        if (uri.Scheme == "app")
        {
           // var navigator = ContainerLocator.Current.Resolve<IRegionManager>();

            //navigator.RequestNavigate("ContentRegion", url);
        }
        else
        {
            Link.OpenLink(url);
        }
    }
}
