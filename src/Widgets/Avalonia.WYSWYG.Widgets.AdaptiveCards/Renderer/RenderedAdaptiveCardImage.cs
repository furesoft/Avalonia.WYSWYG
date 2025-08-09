// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using AdaptiveCards.Rendering;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public class RenderedAdaptiveCardImage : RenderedAdaptiveCardBase
{
    public RenderedAdaptiveCardImage(Stream stream, AdaptiveCard originatingCard, IList<AdaptiveWarning> warnings) :
        base(originatingCard, warnings)
    {
        ImageStream = stream;
    }

    /// <summary>
    ///     The rendered image stream as image/png
    /// </summary>
    public Stream ImageStream { get; }
}