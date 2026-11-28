using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.App.ViewModels;

/// <summary>
/// Textos localizados de las secciones Rendimiento y Widgets de la pantalla
/// de configuración. Agrupados acá (en vez de una propiedad con backing field
/// por cada texto en SettingsViewModel) y refrescados todos juntos al cambiar
/// de idioma o de modo de rendimiento.
/// </summary>
public class SettingsTexts : ViewModelBase
{
    public string PerformanceHeader { get; private set; } = string.Empty;
    public string PerformanceLow { get; private set; } = string.Empty;
    public string PerformanceHigh { get; private set; } = string.Empty;
    public string PerformanceHint { get; private set; } = string.Empty;

    public string WidgetsHeader { get; private set; } = string.Empty;
    public string WidgetsHint { get; private set; } = string.Empty;
    public string ClockWidget { get; private set; } = string.Empty;
    public string ClockSeconds { get; private set; } = string.Empty;
    public string Clock24Hour { get; private set; } = string.Empty;
    public string ClockDate { get; private set; } = string.Empty;
    public string DayProgressWidget { get; private set; } = string.Empty;
    public string WidgetCard { get; private set; } = string.Empty;
    public string WidgetSize { get; private set; } = string.Empty;
    public string SizeSmall { get; private set; } = string.Empty;
    public string SizeMedium { get; private set; } = string.Empty;
    public string SizeLarge { get; private set; } = string.Empty;

    public void Refresh(ILocalizationService localization, PerformanceMode mode)
    {
        PerformanceHeader = localization.Get("settings.section.performance");
        PerformanceLow = localization.Get("settings.performance.low");
        PerformanceHigh = localization.Get("settings.performance.high");
        PerformanceHint = localization.Get(mode == PerformanceMode.Low
            ? "settings.performance.lowHint"
            : "settings.performance.highHint");

        WidgetsHeader = localization.Get("settings.section.widgets");
        WidgetsHint = localization.Get("settings.widgets.hint");
        ClockWidget = localization.Get("settings.widget.clock");
        ClockSeconds = localization.Get("settings.widget.seconds");
        Clock24Hour = localization.Get("settings.widget.hour24");
        ClockDate = localization.Get("settings.widget.date");
        DayProgressWidget = localization.Get("settings.widget.dayProgress");
        WidgetCard = localization.Get("settings.widget.card");
        WidgetSize = localization.Get("settings.widget.size");
        SizeSmall = localization.Get("settings.widget.size.small");
        SizeMedium = localization.Get("settings.widget.size.medium");
        SizeLarge = localization.Get("settings.widget.size.large");

        // Cadena vacía = "cambiaron todas las propiedades".
        OnPropertyChanged(string.Empty);
    }
}
