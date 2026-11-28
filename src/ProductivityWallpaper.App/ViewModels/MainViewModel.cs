using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.App.Widgets;
using ProductivityWallpaper.Core.Enums;
using ProductivityWallpaper.Core.Interfaces;
using ProductivityWallpaper.Core.Models;
using System.IO;
using System.Windows.Threading;

namespace ProductivityWallpaper.App.ViewModels;

/// <summary>
/// Controla el flujo Bienvenida → Idioma → Intereses → Estilo → Frecuencia →
/// Resumen → Vista previa. Mantiene un borrador de UserPreferences que se va
/// completando a medida que el usuario avanza, y se conserva al volver atrás.
/// También gobierna el temporizador de rotación automática (Fase 4): cuenta
/// regresiva en tiempo real, pausar/reanudar y cambio automático al agotarse
/// el intervalo elegido.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly ILocalizationService _localization;
    private readonly IPreferencesStore _preferencesStore;
    private readonly IWallpaperSelector _wallpaperSelector;
    private readonly IWallpaperService _wallpaperService;
    private readonly IStartupService _startupService;
    private readonly UserPreferences _draft = new();
    private readonly WidgetManager _widgets;

    private DispatcherTimer? _rotationTimer;
    private int _remainingSeconds;
    private WallpaperPreviewViewModel? _previewViewModel;

    private object _currentViewModel = null!;

    public object CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public MainViewModel(
        ILocalizationService localization,
        IPreferencesStore preferencesStore,
        IWallpaperSelector wallpaperSelector,
        IWallpaperService wallpaperService,
        IStartupService startupService)
    {
        _localization = localization;
        _preferencesStore = preferencesStore;
        _wallpaperSelector = wallpaperSelector;
        _wallpaperService = wallpaperService;
        _startupService = startupService;
        _widgets = new WidgetManager(_draft, _localization, () => _preferencesStore.Save(_draft));

        // Si ya hay preferencias completas guardadas de una sesión anterior,
        // vamos directo a la vista previa en vez de repetir el onboarding.
        var saved = _preferencesStore.Load();
        if (saved is not null && saved.SelectedStyle.HasValue && saved.WallpaperInterval.HasValue)
        {
            _draft.Language = saved.Language;
            _draft.SelectedCategories = saved.SelectedCategories;
            _draft.SelectedStyle = saved.SelectedStyle;
            _draft.WallpaperInterval = saved.WallpaperInterval;
            _draft.AutoChange = saved.AutoChange;
            _draft.IsPaused = saved.IsPaused;
            _draft.Performance = saved.Performance;
            _draft.ShowClockWidget = saved.ShowClockWidget;
            _draft.ClockShowSeconds = saved.ClockShowSeconds;
            _draft.Clock24Hour = saved.Clock24Hour;
            _draft.ClockShowDate = saved.ClockShowDate;
            _draft.ShowDayProgressWidget = saved.ShowDayProgressWidget;
            _draft.WidgetSize = saved.WidgetSize;
            _draft.WidgetCardBackground = saved.WidgetCardBackground;
            _draft.ClockWidgetLeft = saved.ClockWidgetLeft;
            _draft.ClockWidgetTop = saved.ClockWidgetTop;
            _draft.DayProgressWidgetLeft = saved.DayProgressWidgetLeft;
            _draft.DayProgressWidgetTop = saved.DayProgressWidgetTop;
            PerformanceProfile.Apply(_draft.Performance);
            // El registro de Windows es la fuente de verdad: si el usuario
            // borró la entrada a mano (o la máquina es otra), la preferencia
            // guardada no debe mentirle a la UI.
            _draft.StartWithWindows = _startupService.IsEnabled();
            _localization.CurrentLanguage = _draft.Language;
            ShowWallpaperPreview();
        }
        else
        {
            ShowWelcome();
        }
    }

    /// <summary>
    /// Fuerza un cambio de wallpaper inmediato (usado por "Cambiar ahora" y
    /// también por el menú de la bandeja del sistema).
    /// </summary>
    public void TriggerChangeNow()
    {
        if (CurrentViewModel is WallpaperPreviewViewModel)
        {
            ShowWallpaperPreview();
        }
    }

    /// <summary>
    /// Alterna pausar/reanudar desde fuera de la pantalla (menú de la
    /// bandeja). No hace nada si todavía no se completó el onboarding o si
    /// la frecuencia elegida es Manual (no hay nada para pausar).
    /// </summary>
    public void ToggleRotationFromTray()
    {
        if (_previewViewModel is null || !_previewViewModel.ShowRotationControls)
        {
            return;
        }

        TogglePause(!_draft.IsPaused);
        _previewViewModel.ApplyExternalPauseState(_draft.IsPaused);
    }

    public bool IsRotationPaused => _draft.IsPaused;
    public bool HasRotationControls => _previewViewModel?.ShowRotationControls ?? false;

    private void ShowWelcome()
    {
        CurrentViewModel = new WelcomeViewModel(_localization, onStart: ShowLanguage);
    }

    private void ShowLanguage()
    {
        CurrentViewModel = new LanguageViewModel(_localization, onSelected: OnLanguageSelected, onBack: ShowWelcome);
    }

    private void OnLanguageSelected(AppLanguage language)
    {
        _draft.Language = language;
        _localization.CurrentLanguage = language;
        ShowInterests();
    }

    private void ShowInterests()
    {
        CurrentViewModel = new InterestsViewModel(
            _localization,
            _draft.SelectedCategories,
            onContinue: OnInterestsConfirmed,
            onBack: ShowLanguage);
    }

    private void OnInterestsConfirmed(List<InterestCategory> categories)
    {
        _draft.SelectedCategories = categories;
        ShowStyle();
    }

    private void ShowStyle()
    {
        CurrentViewModel = new StyleViewModel(
            _localization,
            _draft.SelectedStyle,
            onContinue: OnStyleSelected,
            onBack: ShowInterests);
    }

    private void OnStyleSelected(VisualStyle style)
    {
        _draft.SelectedStyle = style;
        ShowFrequency();
    }

    private void ShowFrequency()
    {
        CurrentViewModel = new FrequencyViewModel(
            _localization,
            _draft.WallpaperInterval,
            onContinue: OnFrequencySelected,
            onBack: ShowStyle);
    }

    private void OnFrequencySelected(WallpaperFrequency frequency)
    {
        _draft.WallpaperInterval = frequency;
        _draft.AutoChange = frequency != WallpaperFrequency.Manual;
        ShowSummary();
    }

    private void ShowSummary()
    {
        CurrentViewModel = new SummaryViewModel(_localization, _draft, onConfirm: OnConfirmed, onBack: ShowFrequency);
    }

    private void OnConfirmed()
    {
        ShowWallpaperPreview();
    }

    /// <summary>
    /// Pantalla de configuración (accesible desde el botón "Configuración"
    /// en la vista principal): una sola pantalla agrupada estilo "Ajustes"
    /// de iOS (idioma, intereses, estilo, frecuencia, inicio con Windows),
    /// en vez del wizard de 3 pasos de antes. Los cambios se guardan en el
    /// mismo draft a medida que se tocan; "Listo" recalcula y aplica el
    /// wallpaper por si algo cambió, y vuelve a la vista previa.
    /// </summary>
    public void OpenSettings()
    {
        CurrentViewModel = new SettingsViewModel(
            _localization,
            _draft,
            onDone: ShowWallpaperPreview,
            onToggleStartWithWindows: ToggleStartWithWindows,
            onPerformanceChanged: OnPerformanceChanged,
            onWidgetsChanged: OnWidgetsChanged);
    }

    /// <summary>Libera los widgets de escritorio (cierra sus ventanas). Se llama al salir de la app.</summary>
    public void Shutdown()
    {
        _widgets.Dispose();
    }

    private void OnPerformanceChanged()
    {
        PerformanceProfile.Apply(_draft.Performance);
        _widgets.Refresh();
        _preferencesStore.Save(_draft);
    }

    private void OnWidgetsChanged()
    {
        _widgets.Refresh();
        _preferencesStore.Save(_draft);
    }

    /// <summary>
    /// Selecciona contenido nuevo acorde a las preferencias actuales y lo
    /// aplica de verdad como fondo de Windows (confirmar onboarding, volver
    /// de "Configuración", reabrir la app, "Cambiar ahora", o el propio
    /// temporizador al agotarse el intervalo). También reinicia la cuenta
    /// regresiva de rotación.
    /// </summary>
    private void ShowWallpaperPreview()
    {
        _localization.CurrentLanguage = _draft.Language;
        _preferencesStore.Save(_draft);

        var content = _wallpaperSelector.SelectNext(_draft);
        var status = ApplyToWindows(content);
        ResetRotationTimer();

        var viewModel = new WallpaperPreviewViewModel(
            _localization,
            content,
            _draft,
            status,
            _remainingSeconds,
            onChangeNow: ShowWallpaperPreview,
            onOpenSettings: OpenSettings,
            onTogglePause: TogglePause,
            onToggleStartWithWindows: ToggleStartWithWindows);

        _previewViewModel = viewModel;
        CurrentViewModel = viewModel;
        _widgets.Refresh();
    }

    private WallpaperApplyStatus ApplyToWindows(WallpaperContent? content)
    {
        if (content is null)
        {
            return WallpaperApplyStatus.NoContent;
        }

        var fullPath = SafePaths.ResolveCatalogImage(content.ImagePath);
        if (fullPath is null || !File.Exists(fullPath))
        {
            return WallpaperApplyStatus.ImageMissing;
        }

        return _wallpaperService.ApplyWallpaper(fullPath) ? WallpaperApplyStatus.Applied : WallpaperApplyStatus.Failed;
    }

    private void ResetRotationTimer()
    {
        var seconds = _draft.WallpaperInterval?.ToSeconds();
        _remainingSeconds = seconds ?? 0;

        if (seconds.HasValue && !_draft.IsPaused)
        {
            StartTimer();
        }
        else
        {
            StopTimer();
        }
    }

    private void StartTimer()
    {
        if (_rotationTimer is null)
        {
            _rotationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _rotationTimer.Tick += OnRotationTick;
        }

        if (!_rotationTimer.IsEnabled)
        {
            _rotationTimer.Start();
        }
    }

    private void StopTimer()
    {
        _rotationTimer?.Stop();
    }

    private void OnRotationTick(object? sender, EventArgs e)
    {
        if (_draft.IsPaused)
        {
            StopTimer();
            return;
        }

        _remainingSeconds--;

        if (_remainingSeconds <= 0)
        {
            // Rotación automática: se cumplió el intervalo elegido.
            ShowWallpaperPreview();
        }
        else
        {
            _previewViewModel?.UpdateCountdown(_remainingSeconds);
        }
    }

    private void TogglePause(bool isPaused)
    {
        _draft.IsPaused = isPaused;
        _preferencesStore.Save(_draft);

        if (isPaused)
        {
            StopTimer();
        }
        else if (_draft.WallpaperInterval.HasValue && _draft.WallpaperInterval.Value != WallpaperFrequency.Manual)
        {
            // Reanudar continúa con el tiempo restante, no reinicia el intervalo completo.
            StartTimer();
        }
    }

    /// <summary>
    /// Activa/desactiva el inicio automático con Windows (Fase 6). Si el
    /// registro falla (por ejemplo, corriendo con "dotnet run" en vez del
    /// .exe publicado), la preferencia vuelve a false para no mostrar un
    /// estado que en realidad no está activo.
    /// </summary>
    private void ToggleStartWithWindows(bool enable)
    {
        var success = enable ? _startupService.Enable() : _startupService.Disable();
        _draft.StartWithWindows = success && enable;
        _preferencesStore.Save(_draft);

        if (enable && !success && _previewViewModel is not null)
        {
            _previewViewModel.StartWithWindows = false;
        }
    }
}
