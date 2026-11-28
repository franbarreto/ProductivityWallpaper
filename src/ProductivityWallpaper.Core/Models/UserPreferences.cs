using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.Core.Models;

/// <summary>
/// Preferencias de configuración elegidas por el usuario en el onboarding.
/// No contiene ninguna lógica de Windows ni de UI.
/// </summary>
public class UserPreferences
{
    public AppLanguage Language { get; set; } = AppLanguage.Es;

    public List<InterestCategory> SelectedCategories { get; set; } = new();

    public VisualStyle? SelectedStyle { get; set; }

    public WallpaperFrequency? WallpaperInterval { get; set; }

    /// <summary>
    /// false cuando la frecuencia elegida es Manual.
    /// </summary>
    public bool AutoChange { get; set; } = true;

    /// <summary>
    /// true cuando el usuario pausó manualmente el cambio automático desde
    /// la pantalla principal. Se conserva al cerrar y reabrir la app.
    /// </summary>
    public bool IsPaused { get; set; } = false;

    /// <summary>
    /// true si la app debe iniciarse automáticamente al iniciar sesión en
    /// Windows (Fase 6). El registro real en el sistema lo gestiona
    /// IStartupService; este flag solo refleja la preferencia del usuario
    /// para poder mostrarla en la UI sin tener que leer el registro de Windows.
    /// </summary>
    public bool StartWithWindows { get; set; } = false;

    /// <summary>
    /// Perfil de recursos: Low reduce efectos, animaciones, calidad de
    /// imagen en memoria y frecuencia de actualización de los widgets.
    /// </summary>
    public PerformanceMode Performance { get; set; } = PerformanceMode.High;

    // --- Widgets de escritorio ---

    public bool ShowClockWidget { get; set; } = false;

    /// <summary>Se ignora en modo de bajo rendimiento (actualizar cada segundo cuesta).</summary>
    public bool ClockShowSeconds { get; set; } = false;

    public bool Clock24Hour { get; set; } = true;

    public bool ClockShowDate { get; set; } = true;

    public bool ShowDayProgressWidget { get; set; } = false;

    public WidgetSize WidgetSize { get; set; } = WidgetSize.Medium;

    /// <summary>true: tarjeta translúcida detrás del texto; false: texto directo sobre el wallpaper.</summary>
    public bool WidgetCardBackground { get; set; } = true;

    // Última posición (en pantalla) de cada widget; null = posición por defecto.
    public double? ClockWidgetLeft { get; set; }
    public double? ClockWidgetTop { get; set; }
    public double? DayProgressWidgetLeft { get; set; }
    public double? DayProgressWidgetTop { get; set; }
}
