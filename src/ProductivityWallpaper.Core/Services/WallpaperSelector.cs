using ProductivityWallpaper.Core.Interfaces;
using ProductivityWallpaper.Core.Models;

namespace ProductivityWallpaper.Core.Services;

/// <summary>
/// Filtrado por niveles (idioma+categoría+estilo → idioma+categoría → idioma
/// → cualquiera), sin repetir los últimos elementos mostrados mientras haya
/// alternativas, con selección aleatoria entre los candidatos resultantes.
/// El idioma nunca se relaja: siempre tiene prioridad sobre estilo/categoría.
/// </summary>
public class WallpaperSelector : IWallpaperSelector
{
    private const int HistorySize = 5;

    private readonly IWallpaperRepository _repository;
    private readonly Random _random = new();
    private readonly List<string> _recentlyShown = new();

    public WallpaperSelector(IWallpaperRepository repository)
    {
        _repository = repository;
    }

    public WallpaperContent? SelectNext(UserPreferences preferences)
    {
        var all = _repository.GetAll();
        if (all.Count == 0)
        {
            return null;
        }

        var candidates = GetCandidatesWithFallback(all, preferences);
        if (candidates.Count == 0)
        {
            return null;
        }

        var selected = PickAvoidingRecent(candidates);
        RememberShown(selected.Id);
        return selected;
    }

    private static List<WallpaperContent> GetCandidatesWithFallback(
        IReadOnlyList<WallpaperContent> all,
        UserPreferences preferences)
    {
        // Nivel 1: idioma + categoría + estilo
        var level1 = all.Where(w =>
                MatchesLanguage(w, preferences) &&
                MatchesCategories(w, preferences) &&
                MatchesStyle(w, preferences))
            .ToList();
        if (level1.Count > 0)
        {
            return level1;
        }

        // Nivel 2: idioma + categoría
        var level2 = all.Where(w => MatchesLanguage(w, preferences) && MatchesCategories(w, preferences)).ToList();
        if (level2.Count > 0)
        {
            return level2;
        }

        // Nivel 3: idioma
        var level3 = all.Where(w => MatchesLanguage(w, preferences)).ToList();
        if (level3.Count > 0)
        {
            return level3;
        }

        // Nivel 4: cualquier contenido disponible (el idioma no tiene contenido propio)
        return all.ToList();
    }

    private static bool MatchesLanguage(WallpaperContent content, UserPreferences preferences)
        => content.Language == preferences.Language;

    private static bool MatchesCategories(WallpaperContent content, UserPreferences preferences)
        => preferences.SelectedCategories.Count == 0
           || content.Categories.Any(preferences.SelectedCategories.Contains);

    private static bool MatchesStyle(WallpaperContent content, UserPreferences preferences)
        => preferences.SelectedStyle is null || content.Style == preferences.SelectedStyle;

    private WallpaperContent PickAvoidingRecent(List<WallpaperContent> candidates)
    {
        var notRecentlyShown = candidates.Where(c => !_recentlyShown.Contains(c.Id)).ToList();

        // Si ya mostramos todos los compatibles, reiniciamos el ciclo usando
        // el conjunto completo de candidatos.
        var pool = notRecentlyShown.Count > 0 ? notRecentlyShown : candidates;

        return pool[_random.Next(pool.Count)];
    }

    private void RememberShown(string id)
    {
        _recentlyShown.Add(id);
        while (_recentlyShown.Count > HistorySize)
        {
            _recentlyShown.RemoveAt(0);
        }
    }
}
