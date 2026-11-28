using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.App.Presentation;

/// <summary>
/// Mientras no haya imágenes reales, la tarjeta de vista previa usa un color
/// de fondo asociado al estilo elegido, con la frase superpuesta. El día que
/// haya wallpapers reales, esto queda solo como respaldo si falta la imagen.
/// </summary>
public static class StylePalette
{
    public static string BackgroundHex(VisualStyle style) => style switch
    {
        VisualStyle.Minimalist => "#2A2D3A",
        VisualStyle.Dark => "#0B0C10",
        VisualStyle.Light => "#E8EAF0",
        VisualStyle.Nature => "#1F3B2C",
        VisualStyle.Abstract => "#4B2E83",
        VisualStyle.Elegant => "#3A2E1F",
        _ => "#2A2D3A"
    };

    public static string TextHex(VisualStyle style) => style == VisualStyle.Light ? "#1A1D29" : "#F5F5F7";
}
