using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.App.Services;

/// <summary>
/// Traduce claves de texto al idioma actual. Las pantallas nunca deben tener
/// texto hardcodeado: siempre piden la traducción por clave a este servicio.
/// </summary>
public interface ILocalizationService
{
    AppLanguage CurrentLanguage { get; set; }

    string Get(string key);
}
