using Avalonia.Controls;
using Avalonia.Data;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards;

public class AdaptiveCardEditTemplate : WidgetTemplate
{
    public override Control Build(WidgetModel model)
    {
        var def = @"{
  ""type"": ""AdaptiveCard"",
  ""body"": [
    {
      ""type"": ""TextBlock"",
      ""size"": ""medium"",
      ""weight"": ""bolder"",
      ""text"": ""${title}"",
      ""style"": ""heading"",
      ""wrap"": true
    },
    {
      ""type"": ""ColumnSet"",
      ""columns"": [
        {
          ""type"": ""Column"",
          ""items"": [
            {
              ""type"": ""Image"",
              ""style"": ""person"",
              ""url"": ""${creator.profileImage}"",
              ""altText"": ""${creator.name}"",
              ""size"": ""small""
            }
          ],
          ""width"": ""auto""
        },
        {
          ""type"": ""Column"",
          ""items"": [
            {
              ""type"": ""TextBlock"",
              ""weight"": ""bolder"",
              ""text"": ""${creator.name}"",
              ""wrap"": true
            },
            {
              ""type"": ""TextBlock"",
              ""spacing"": ""none"",
              ""text"": ""Created {{DATE(${string(createdUtc)}, SHORT)}}"",
              ""isSubtle"": true,
              ""wrap"": true
            }
          ],
          ""width"": ""stretch""
        }
      ]
    },
    {
      ""type"": ""TextBlock"",
      ""text"": ""${description}"",
      ""wrap"": true
    },
    {
      ""type"": ""FactSet"",
      ""facts"": [
        {
          ""$data"": ""${properties}"",
          ""title"": ""${key}:"",
          ""value"": ""${value}""
        }
      ]
    }
  ],
  ""actions"": [
    {
      ""type"": ""Action.ShowCard"",
      ""title"": ""Set due date"",
      ""card"": {
        ""type"": ""AdaptiveCard"",
        ""body"": [
          {
            ""type"": ""Input.Date"",
            ""label"": ""Enter the due date"",
            ""id"": ""dueDate""
          },
          {
            ""type"": ""Input.Text"",
            ""id"": ""comment"",
            ""isMultiline"": true,
            ""label"": ""Add a comment""
          }
        ],
        ""actions"": [
          {
            ""type"": ""Action.Submit"",
            ""title"": ""OK""
          }
        ],
        ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json""
      }
    },
    {
      ""type"": ""Action.OpenUrl"",
      ""title"": ""View"",
      ""url"": ""${viewUrl}"",
      ""role"": ""button""
    }
  ],
  ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
  ""version"": ""1.5""
}
";

        model.SetProperty("Definition", def);

        var textBox = new TextBox
        {
            MinHeight = 150,
            MinWidth = 150,
            MaxHeight = 350,
            AcceptsReturn = true,
            [!TextBox.TextProperty] = new Binding("Properties[Definition]", BindingMode.TwoWay)
        };

        return textBox;
    }
}