using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MistTrader.UI.Converters;

public class TradeMakerToIconConverter : IValueConverter
{
    public static StreamGeometry ArrowCircleUpRegularIcon { get; internal set; } = null!;
    public static StreamGeometry ArrowCircleDownRegularIcon { get; internal set; } = null!;
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string type)
            return ArrowCircleUpRegularIcon;

        return type switch
        {
            "Buy" => ArrowCircleDownRegularIcon,
            "Sell" => ArrowCircleUpRegularIcon,
            _ => ArrowCircleUpRegularIcon
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //empty
        
        return AvaloniaProperty.UnsetValue;
    }
}