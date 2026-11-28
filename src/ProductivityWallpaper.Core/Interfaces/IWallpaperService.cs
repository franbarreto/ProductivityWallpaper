namespace ProductivityWallpaper.Core.Interfaces;

/// <summary>
/// Aplica una imagen como fondo de escritorio real. La implementación
/// concreta (Windows, vía P/Invoke) vive en Infrastructure; Core no sabe
/// nada de Win32.
/// </summary>
public interface IWallpaperService
{
    /// <summary>
    /// Intenta aplicar la imagen indicada (ruta absoluta) como fondo de
    /// escritorio. Devuelve false si no se pudo (por ejemplo, si el archivo
    /// no existe), sin lanzar excepción.
    /// </summary>
    bool ApplyWallpaper(string absoluteImagePath);
}
