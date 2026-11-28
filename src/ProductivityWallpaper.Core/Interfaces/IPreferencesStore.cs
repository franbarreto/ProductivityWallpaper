using ProductivityWallpaper.Core.Models;

namespace ProductivityWallpaper.Core.Interfaces;

/// <summary>
/// Abstracción para guardar y recuperar las preferencias del usuario.
/// En la Fase 1 se implementa en memoria; en una fase posterior se podrá
/// reemplazar por una implementación en disco sin tocar Core ni App.
/// </summary>
public interface IPreferencesStore
{
    void Save(UserPreferences preferences);

    UserPreferences? Load();
}
