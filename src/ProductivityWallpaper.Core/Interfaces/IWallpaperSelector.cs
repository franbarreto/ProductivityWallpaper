using ProductivityWallpaper.Core.Models;

namespace ProductivityWallpaper.Core.Interfaces;

/// <summary>
/// Recibe UserPreferences y devuelve el próximo WallpaperContent a mostrar,
/// aplicando idioma/categorías/estilo, fallback por niveles y anti-repetición.
/// </summary>
public interface IWallpaperSelector
{
    WallpaperContent? SelectNext(UserPreferences preferences);
}
