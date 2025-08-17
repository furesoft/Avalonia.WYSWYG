using Avalonia.WYSWYG.Parsing;
using Splat;

namespace Avalonia.WYSWYG;

public static class AppBuilderExtensions
{
    public static AppBuilder WithWyswyg(this AppBuilder builder, Action<WyswygOptions> configureOptions)
    {
        builder.AfterSetup(_ =>
        {
            var options = new WyswygOptions();
            configureOptions(options);
            Locator.CurrentMutable.RegisterConstant(options);
            Locator.CurrentMutable.RegisterConstant(options.WidgetStorage);
            Locator.CurrentMutable.RegisterConstant(new PropertiesMetaExtractor());
        });

        return builder;
    }
}