﻿using System.Globalization;
using Avalonia.Data;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Converters;

internal class EnumToCheckedConverter : ValueConverter<EnumToCheckedConverter, bool>
{
    public new object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Equals(value, parameter);
    }

    protected override object Convert(bool isChecked, Type targetType, object parameter)
    {
        return isChecked ? parameter : BindingOperations.DoNothing;
    }
}
