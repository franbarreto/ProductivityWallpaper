using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProductivityWallpaper.App.Services;

/// <summary>
/// Evita releer/redecodificar del disco una imagen que ya se mostró antes
/// en esta sesión, y decodifica a un ancho razonable para una tarjeta de
/// vista previa en vez de cargar el archivo a resolución completa.
/// </summary>
public static class ImageCache
{
    // El ancho de decodificación y el tamaño del caché dependen del modo de
    // rendimiento (ver PerformanceProfile): en alto 1280 px (nítido incluso
    // en pantallas de alta densidad); en bajo 640 px, que alcanza de sobra
    // para la tarjeta de vista previa y ocupa 4 veces menos memoria.
    private static readonly Dictionary<string, ImageBrush> Cache = new();
    private static readonly Queue<string> Order = new();

    /// <summary>Vacía el caché (al cambiar de modo, para volver a decodificar al nuevo tamaño).</summary>
    public static void Clear()
    {
        Cache.Clear();
        Order.Clear();
    }

    public static ImageBrush? GetOrLoad(string absoluteImagePath)
    {
        if (Cache.TryGetValue(absoluteImagePath, out var cached))
        {
            return cached;
        }

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelWidth = PerformanceProfile.ImageDecodeWidth;
            bitmap.UriSource = new Uri(absoluteImagePath, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();

            // Uniform (no UniformToFill): las imágenes son panorámicas
            // (16:9) pero la tarjeta de vista previa es más angosta y alta,
            // así que recortar para llenarla ("UniformToFill") se comía los
            // costados de la imagen, justo donde suele estar la frase —
            // se veía "cortada"/desplazada. Con Uniform se ve siempre
            // completa, con el color del estilo como marco si no coincide
            // el aspecto.
            var brush = new ImageBrush(bitmap) { Stretch = Stretch.Uniform };
            brush.Freeze();

            // Caché acotada: descarta la imagen más antigua al llenarse, para
            // que la memoria no crezca con cada wallpaper distinto mostrado.
            while (Order.Count >= PerformanceProfile.ImageCacheCapacity)
            {
                Cache.Remove(Order.Dequeue());
            }

            Cache[absoluteImagePath] = brush;
            Order.Enqueue(absoluteImagePath);
            return brush;
        }
        catch (Exception ex) when (ex is NotSupportedException or IOException or UriFormatException)
        {
            return null;
        }
    }
}
