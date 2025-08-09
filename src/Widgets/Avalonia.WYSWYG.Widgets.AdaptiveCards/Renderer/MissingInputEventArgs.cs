// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using Avalonia.Controls;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public class MissingInputEventArgs : EventArgs
{
    public MissingInputEventArgs(AdaptiveInput input, Control frameworkElement)
    {
        Control = frameworkElement;
        AdaptiveInput = input;
    }

    public Control Control { get; private set; }

    public AdaptiveInput AdaptiveInput { get; private set; }
}