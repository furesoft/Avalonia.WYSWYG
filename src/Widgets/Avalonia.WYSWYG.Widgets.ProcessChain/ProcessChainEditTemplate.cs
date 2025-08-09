using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.ViewModels;
using MainWindow = Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Views.MainWindow;

namespace Avalonia.WYSWYG.Widgets.ProcessChain;

public class ProcessChainEditTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var btn = new Button {Content = "Edit"};

        btn.Click += (s, e) =>
        {
            new MainWindow(){DataContext = new MainViewViewModel()}.Show();
        };

        return btn;
    }
}
