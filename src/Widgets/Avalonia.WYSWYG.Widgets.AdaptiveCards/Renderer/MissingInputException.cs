// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using Avalonia.Controls;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public class MissingInputException : Exception
{
    public MissingInputException(string message, AdaptiveInput input, Control frameworkElement)
        : base(message)
    {
        Control = frameworkElement;
        AdaptiveInput = input;
    }

    public Control Control { get; set; }

    public AdaptiveInput AdaptiveInput { get; set; }
}