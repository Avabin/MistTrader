using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace MistTrader.UI.Converters;

public class StatusColorConverter : IValueConverter
{
    public static readonly StatusColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isRunning)
            return null;
            
        return isRunning ? 
            Application.Current?.Resources["RunningColor"] : 
            Application.Current?.Resources["StoppedColor"];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}