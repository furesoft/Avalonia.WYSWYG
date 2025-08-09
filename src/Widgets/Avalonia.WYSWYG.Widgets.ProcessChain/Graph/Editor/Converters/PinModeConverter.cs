﻿using Avalonia.Media;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Converters;

public class PinModeConverter : ValueConverter<PinModeConverter, PinMode>
{
    protected override object Convert(PinMode mode, Type targetType, object parameter)
    {
        return mode switch
        {
            PinMode.Input => Brushes.Green,
            PinMode.Output => Brushes.Red,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
