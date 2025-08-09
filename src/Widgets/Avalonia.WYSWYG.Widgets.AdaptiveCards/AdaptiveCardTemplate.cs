using AdaptiveCards;
using AdaptiveCards.Rendering;
using Avalonia.Controls;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards;

public class AdaptiveCardTemplate : WidgetTemplate
{
    /*
     AdaptiveExpressions.Expression.Functions.Add("stringFormat", (args) =>
       {
           string formattedString = "";

           // argument is packed in sequential order as defined in the template
           // For example, suppose we have "${stringFormat(strings.myName, person.firstName, person.lastName)}"
           // args will have following entries
           // args[0]: strings.myName
           // args[1]: person.firstName
           // args[2]: strings.lastName
           if (args[0] != null && args[1] != null && args[2] != null)
           {
               string formatString = args[0];
               string[] stringArguments = {args[1], args[2] };
               formattedString = string.Format(formatString, stringArguments);
           }
           return formattedString;
       });
     */

    public override Control Build(WidgetModel model)
    {
        var def = model.GetProperty<string>("Definition");
        var template = new global::AdaptiveCards.Templating.AdaptiveCardTemplate(def);

        var myData = new
        {
            Title = "Usefull Title"
        };

        var card = AdaptiveCard.FromJson(template.Expand(myData));

        var hostConfigDef = GetHostConfig();
        var hostConfig = AdaptiveHostConfig.FromJson(hostConfigDef);

        var renderer = new AdaptiveCardRenderer(hostConfig);
        var renderedCard = renderer.RenderCard(card.Card);
        renderedCard.OnAction += (sender, e) =>
        {
            // handle event...
        };

        return renderedCard.Control;
    }

    private static string GetHostConfig()
    {
        var assembly = typeof(AdaptiveCardTemplate).Assembly;
        var themeVariant = Application.Current!.ActualThemeVariant!.Key!.ToString()!.ToLower();
        var resourceStream = assembly.GetManifestResourceStream(
            $"{typeof(AdaptiveCardTemplate).Namespace}.Configs.widget-{themeVariant}.json");

        return new StreamReader(resourceStream!).ReadToEnd();
    }
}