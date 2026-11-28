using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.Core.Models;

/// <summary>
/// Un elemento del catálogo local: una frase asociada a idioma, categorías y
/// estilo, con la ruta (relativa) de su imagen. Sin dependencias de Windows.
/// </summary>
public class WallpaperContent
{
    public required string Id { get; init; }

    public required AppLanguage Language { get; init; }

    public required string Quote { get; init; }

    /// <summary>
    /// null cuando la frase es de autoría propia / no corresponde atribuirla.
    /// </summary>
    public string? Author { get; init; }

    public required List<InterestCategory> Categories { get; init; }

    public required VisualStyle Style { get; init; }

    /// <summary>
    /// Ruta relativa (nunca absoluta de Windows) a la imagen del wallpaper.
    /// </summary>
    public required string ImagePath { get; init; }
}
