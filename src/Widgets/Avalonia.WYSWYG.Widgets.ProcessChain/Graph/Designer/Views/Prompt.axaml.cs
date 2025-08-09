using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.ViewModels;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Views;

public partial class Prompt : Window
{
    public Prompt()
    {
        InitializeComponent();
    }

    public static Task<string> Show(string title, string hint, string input = null)
    {
        var window = new Prompt();
        var viewModel = new PromptViewModel();
        var tcs = new TaskCompletionSource<string>();

        viewModel.Title = title;
        viewModel.Input = input;
        viewModel.Hint = hint;

        window.DataContext = viewModel;

        var btn = window.FindControl<Button>("OkButton");
        btn.Click += (sender, args) =>
        {
            tcs.SetResult(viewModel.Input);
            window.Close();
        };
        window.Show();

        return tcs.Task;
    }
}
