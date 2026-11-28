using ProductivityWallpaper.Core.Models;

namespace ProductivityWallpaper.Core.Interfaces;

/// <summary>
/// Acceso a los datos del catálogo. La UI nunca la usa directamente: toda
/// la lógica de filtrado/fallback/anti-repetición vive en IWallpaperSelector.
/// </summary>
public interface IWallpaperRepository
{
    IReadOnlyList<WallpaperContent> GetAll();

    WallpaperContent? GetById(string id);
}
