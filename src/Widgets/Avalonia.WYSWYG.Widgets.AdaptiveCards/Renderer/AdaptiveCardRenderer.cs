// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Diagnostics;
using AdaptiveCards;
using AdaptiveCards.Rendering;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer.Helpers;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public class MyCachedImageLoader : BaseWebImageLoader
{
    private readonly ConcurrentDictionary<string, Task<Bitmap>> _memoryCache = new();

    /// <inheritdoc />
    public MyCachedImageLoader()
    {
    }

    /// <inheritdoc />
    public MyCachedImageLoader(HttpClient httpClient, bool disposeHttpClient) : base(httpClient,
        disposeHttpClient)
    {
    }

    /// <inheritdoc />
    public override async Task<Bitmap> ProvideImageAsync(string url)
    {
        var bitmap = await _memoryCache.GetOrAdd(url, LoadAsync).ConfigureAwait(false);
        // If load failed - remove from cache and return
        // Next load attempt will try to load image again
        if (bitmap == null) _memoryCache.TryRemove(url, out _);
        return bitmap;
    }

    protected override async Task<Bitmap> LoadAsync(string url)
    {
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            var array = await LoadDataFromExternalAsync(url).ConfigureAwait(false);
            if (array == null) return null;

            using var memoryStream = new MemoryStream(array);
            var bitmap = new Bitmap(memoryStream);
            await SaveToGlobalCache(url, array).ConfigureAwait(false);

            sw.Stop();
            if (Debugger.IsAttached)
                Debug.WriteLine($"Image {url} loaded in {sw.ElapsedMilliseconds} ms");
            return bitmap;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public class AdaptiveCardRenderer : AdaptiveCardRendererBase<Control, AdaptiveRenderContext>
{
    protected Action<object, AdaptiveActionEventArgs> ActionCallback;
    protected Action<object, MissingInputEventArgs> missingDataCallback;

    static AdaptiveCardRenderer()
    {
        ImageLoader.AsyncImageLoader = new MyCachedImageLoader();
    }

    public AdaptiveCardRenderer() : this(new())
    {
    }

    public AdaptiveCardRenderer(AdaptiveHostConfig hostConfig)
    {
        HostConfig = hostConfig ?? new AdaptiveHostConfig();
        SetObjectTypes();
    }

    public AdaptiveFeatureRegistration FeatureRegistration { get; } = new();

    public AdaptiveActionHandlers ActionHandlers { get; } = new();

    protected override AdaptiveSchemaVersion GetSupportedSchemaVersion()
    {
        return new(1, 5);
    }

    private void SetObjectTypes()
    {
        ElementRenderers.Set<AdaptiveCard>(RenderAdaptiveCardWrapper);

        ElementRenderers.Set<AdaptiveTextBlock>(AdaptiveTextBlockRenderer.Render);
        ElementRenderers.Set<AdaptiveRichTextBlock>(AdaptiveRichTextBlockRenderer.Render);

        ElementRenderers.Set<AdaptiveImage>(AdaptiveImageRenderer.Render);
        ElementRenderers.Set<AdaptiveMedia>(AdaptiveMediaRenderer.Render);

        ElementRenderers.Set<AdaptiveContainer>(AdaptiveContainerRenderer.Render);
        ElementRenderers.Set<AdaptiveColumn>(AdaptiveColumnRenderer.Render);
        ElementRenderers.Set<AdaptiveColumnSet>(AdaptiveColumnSetRenderer.Render);
        ElementRenderers.Set<AdaptiveFactSet>(AdaptiveFactSetRenderer.Render);
        ElementRenderers.Set<AdaptiveImageSet>(AdaptiveImageSetRenderer.Render);
        ElementRenderers.Set<AdaptiveActionSet>(AdaptiveActionSetRenderer.Render);

        ElementRenderers.Set<AdaptiveChoiceSetInput>(AdaptiveChoiceSetRenderer.Render);
        ElementRenderers.Set<AdaptiveTextInput>(AdaptiveTextInputRenderer.Render);
        ElementRenderers.Set<AdaptiveNumberInput>(AdaptiveNumberInputRenderer.Render);
        ElementRenderers.Set<AdaptiveDateInput>(AdaptiveDateInputRenderer.Render);
        ElementRenderers.Set<AdaptiveTimeInput>(AdaptiveTimeInputRenderer.Render);
        ElementRenderers.Set<AdaptiveToggleInput>(AdaptiveToggleInputRenderer.Render);

        ElementRenderers.Set<AdaptiveAction>(AdaptiveActionRenderer.Render);

        ElementRenderers.Set<AdaptiveTable>(AdaptiveTableRenderer.Render);
        ElementRenderers.Set<AdaptiveTableCell>(AdaptiveTableCellRenderer.Render);

        ActionHandlers.AddSupportedAction<AdaptiveOverflowAction>();
    }

    public static Control RenderAdaptiveCardWrapper(AdaptiveCard card, AdaptiveRenderContext context)
    {
        var outerGrid = new Grid();
        // outerGrid.Style = context.GetStyle("Adaptive.Card");

        outerGrid.Background = context.GetColorBrush(context.Config.ContainerStyles.Default.BackgroundColor);
        outerGrid.SetBackgroundSource(card.BackgroundImage, context);

        if (context.CardRoot == null) context.CardRoot = outerGrid;

        // Missing schema
        var cardRtl =
            ((IDictionary<string, object>) card.AdditionalProperties).TryGetValue<bool?>("rtl"); //  previousContextRtl;
        var updatedRtl = false;

        if (cardRtl.HasValue && cardRtl.Value)
        {
            context.Rtl = true;
            updatedRtl = true;
        }

        if (cardRtl.HasValue)
            outerGrid.FlowDirection = cardRtl.Value ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        // Reset the parent style
        context.RenderArgs.ParentStyle = AdaptiveContainerStyle.Default;

        var grid = new Grid();
        // grid.Style = context.GetStyle("Adaptive.InnerCard");
        grid.Margin = new(context.Config.Spacing.Padding);

        grid.ColumnDefinitions.Add(new() {Width = new(1, GridUnitType.Star)});

        RendererUtil.ApplyVerticalContentAlignment(grid, card.VerticalContentAlignment);

        outerGrid.MinHeight = card.PixelMinHeight;

        outerGrid.Children.Add(grid);

        var parentCardId = context.RenderArgs.ContainerCardId;
        if (card.InternalID == null)
            card.InternalID = AdaptiveInternalID.Next();
        context.ParentCards.Add(card.InternalID, parentCardId);
        context.RenderArgs.ContainerCardId = card.InternalID;

        AdaptiveContainerRenderer.AddContainerElements(grid, card.Body, context);
        AdaptiveActionSetRenderer.AddRenderedActions(grid, card.Actions, context, card.InternalID);

        context.RenderArgs.ContainerCardId = parentCardId;

        if (card.SelectAction != null)
        {
            var outerGridWithSelectAction = context.RenderSelectAction(card.SelectAction, outerGrid);

            return outerGridWithSelectAction;
        }

        return outerGrid;
    }

    /// <summary>
    ///     Renders an adaptive card.
    /// </summary>
    /// <param name="card">The card to render</param>
    public RenderedAdaptiveCard RenderCard(AdaptiveCard card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));
        RenderedAdaptiveCard renderCard = null;

        void ActionCallback(object sender, AdaptiveActionEventArgs args)
        {
            renderCard?.InvokeOnAction(args);
        }

        void MediaClickCallback(object sender, AdaptiveMediaEventArgs args)
        {
            renderCard?.InvokeOnMediaClick(args);
        }

        var context = new AdaptiveRenderContext(ActionCallback, null, MediaClickCallback)
        {
            ActionHandlers = ActionHandlers,
            Config = HostConfig ?? new AdaptiveHostConfig(),
            ElementRenderers = ElementRenderers,
            FeatureRegistration = FeatureRegistration,
            Lang = card.Lang,
            RenderArgs = new()
            {
                ForegroundColors = HostConfig != null
                    ? HostConfig.ContainerStyles.Default.ForegroundColors
                    : new ContainerStylesConfig().Default.ForegroundColors
            }
        };

        var element = context.Render(card);
        element.Classes.Add(nameof(AdaptiveCard));

        renderCard = new(element, card, context.Warnings, ref context.InputBindings);

        return renderCard;
    }
}