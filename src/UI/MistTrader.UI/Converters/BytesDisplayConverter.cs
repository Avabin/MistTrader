using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MistTrader.UI.Converters;

/// <summary>
/// Converts a long value (bytes) to selected display format.
/// </summary>
public class BytesDisplayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isNumber = value is long or int or short or byte or ulong or uint or ushort or sbyte;
        if (!isNumber)
        {
            return value;
        }
        var bytes = System.Convert.ToUInt64(value);

        var format = parameter is BytesDisplayFormat displayFormat ? displayFormat : BytesDisplayFormat.Auto;

        if (format == BytesDisplayFormat.Auto)
        {
            // Determine the best format to display the bytes
            format = SelectDisplayFormat(bytes);
            
        }

        return format switch
        {
            BytesDisplayFormat.Bytes => $"{bytes} B",
            BytesDisplayFormat.Kilobytes => $"{bytes / 1024.0:0.##} KB",
            BytesDisplayFormat.Megabytes => $"{bytes / 1024.0 / 1024.0:0.##} MB",
            BytesDisplayFormat.Gigabytes => $"{bytes / 1024.0 / 1024.0 / 1024.0:0.##} GB",
            BytesDisplayFormat.Terabytes => $"{bytes / 1024.0 / 1024.0 / 1024.0 / 1024.0:0.##} TB",
            BytesDisplayFormat.Auto or _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid display format.")
        };
    }
    
    private BytesDisplayFormat SelectDisplayFormat(ulong bytes)
    {
        return bytes switch
        {
            < 1024ul => BytesDisplayFormat.Bytes,
            < 1024ul * 1024 => BytesDisplayFormat.Kilobytes,
            < 1024ul * 1024 * 1024 => BytesDisplayFormat.Megabytes,
            < 1024ul * 1024 * 1024 * 1024 => BytesDisplayFormat.Gigabytes,
            _ => BytesDisplayFormat.Terabytes
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public enum BytesDisplayFormat
{
    Auto,
    Bytes,
    Kilobytes,
    Megabytes,
    Gigabytes,
    Terabytes
}