using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProductivityWallpaper.App.Converters;

public class HexToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrWhiteSpace(hex))
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex)!;
                return new SolidColorBrush(color);
            }
            catch (FormatException)
            {
                // Color inválido: se ignora y se usa el valor por defecto de abajo.
            }
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
