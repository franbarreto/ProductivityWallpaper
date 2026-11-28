using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProductivityWallpaper.App.Services;

namespace ProductivityWallpaper.App.Converters;

/// <summary>
/// values[0]: ruta absoluta de imagen (string?, puede ser null/vacío).
/// values[1]: color hex de respaldo (string).
/// Usado como Background de un Border para que las esquinas redondeadas
/// recorten la imagen igual que recortarían el color. La decodificación y
/// el cacheo de la imagen los maneja ImageCache (evita releer/redecodificar
/// del disco en cada refresh de binding).
/// </summary>
public class ImageOrColorBrushConverter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        var imagePath = values.Length > 0 ? values[0] as string : null;
        var hex = values.Length > 1 ? values[1] as string : null;

        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            var cached = ImageCache.GetOrLoad(imagePath);
            if (cached is not null)
            {
                return cached;
            }
        }

        if (!string.IsNullOrWhiteSpace(hex))
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex)!;
                return new SolidColorBrush(color);
            }
            catch (FormatException)
            {
                // Ignorar y devolver el valor por defecto.
            }
        }

        return Brushes.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
