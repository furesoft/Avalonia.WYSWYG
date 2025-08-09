﻿using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM;

public static class NodeViewModelExtensions
{
    public static IPin AddPin(this NodeViewModel node, (double x, double y) position,
        (double width, double height) size, PinMode mode,
        PinAlignment alignment, string name = null, bool allowMultipleConnections = false)
    {
        var pin = new PinViewModel
        {
            Name = name,
            Parent = node,
            X = position.x,
            Y = position.y,
            Width = size.width,
            Height = size.height,
            Alignment = alignment,
            Mode = mode,
            CanConnectToMultiplePins = allowMultipleConnections
        };

        node.Pins ??= new ObservableCollection<IPin>();
        node.Pins.Add(pin);

        return pin;
    }
}
