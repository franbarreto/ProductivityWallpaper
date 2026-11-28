using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Localization;
using ProductivityWallpaper.App.Presentation;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;
using ProductivityWallpaper.Core.Models;
using System.IO;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

/// <summary>
/// Pantalla principal después del onboarding: muestra el WallpaperContent
/// seleccionado, lo aplica como fondo real de Windows (vía IWallpaperService
/// en MainViewModel) y expone controles para probar el filtrado.
/// </summary>
public class WallpaperPreviewViewModel : ViewModelBase
{
    private readonly ILocalizationService _localization;
    private readonly UserPreferences _preferences;

    private bool _hasContent;
    public bool HasContent
    {
        get => _hasContent;
        private set => SetProperty(ref _hasContent, value);
    }

    private string _noContentMessage = string.Empty;
    public string NoContentMessage
    {
        get => _noContentMessage;
        private set => SetProperty(ref _noContentMessage, value);
    }

    private string _quote = string.Empty;
    public string Quote
    {
        get => _quote;
        private set => SetProperty(ref _quote, value);
    }

    private string _quoteLabel = string.Empty;
    public string QuoteLabel
    {
        get => _quoteLabel;
        private set => SetProperty(ref _quoteLabel, value);
    }

    private bool _hasAuthor;
    public bool HasAuthor
    {
        get => _hasAuthor;
        private set => SetProperty(ref _hasAuthor, value);
    }

    private string _authorText = string.Empty;
    public string AuthorText
    {
        get => _authorText;
        private set => SetProperty(ref _authorText, value);
    }

    private string _categoriesLabel = string.Empty;
    public string CategoriesLabel
    {
        get => _categoriesLabel;
        private set => SetProperty(ref _categoriesLabel, value);
    }

    private string _categoriesValue = string.Empty;
    public string CategoriesValue
    {
        get => _categoriesValue;
        private set => SetProperty(ref _categoriesValue, value);
    }

    private string _styleLabel = string.Empty;
    public string StyleLabel
    {
        get => _styleLabel;
        private set => SetProperty(ref _styleLabel, value);
    }

    private string _styleValue = string.Empty;
    public string StyleValue
    {
        get => _styleValue;
        private set => SetProperty(ref _styleValue, value);
    }

    private bool _hasImage;
    public bool HasImage
    {
        get => _hasImage;
        private set => SetProperty(ref _hasImage, value);
    }

    private string? _imageFullPath;
    public string? ImageFullPath
    {
        get => _imageFullPath;
        private set => SetProperty(ref _imageFullPath, value);
    }

    private string _placeholderColorHex = "#2A2D3A";
    public string PlaceholderColorHex
    {
        get => _placeholderColorHex;
        private set => SetProperty(ref _placeholderColorHex, value);
    }

    private string _placeholderTextColorHex = "#F5F5F7";
    public string PlaceholderTextColorHex
    {
        get => _placeholderTextColorHex;
        private set => SetProperty(ref _placeholderTextColorHex, value);
    }

    private string _changeNowText = string.Empty;
    public string ChangeNowText
    {
        get => _changeNowText;
        private set => SetProperty(ref _changeNowText, value);
    }

    public ICommand ChangeNowCommand { get; }

    private string _statusText = string.Empty;
    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    /// <summary>
    /// true cuando la frecuencia elegida no es Manual: solo entonces tiene
    /// sentido mostrar la cuenta regresiva y el botón Pausar/Reanudar.
    /// </summary>
    private bool _showRotationControls;
    public bool ShowRotationControls
    {
        get => _showRotationControls;
        private set => SetProperty(ref _showRotationControls, value);
    }

    private string _manualModeMessage = string.Empty;
    public string ManualModeMessage
    {
        get => _manualModeMessage;
        private set => SetProperty(ref _manualModeMessage, value);
    }

    private string _nextChangeLabel = string.Empty;
    public string NextChangeLabel
    {
        get => _nextChangeLabel;
        private set => SetProperty(ref _nextChangeLabel, value);
    }

    private string _pausedMessage = string.Empty;
    public string PausedMessage
    {
        get => _pausedMessage;
        private set => SetProperty(ref _pausedMessage, value);
    }

    private string _remainingTimeText = string.Empty;
    public string RemainingTimeText
    {
        get => _remainingTimeText;
        private set => SetProperty(ref _remainingTimeText, value);
    }

    private bool _isPaused;
    public bool IsPaused
    {
        get => _isPaused;
        private set
        {
            if (SetProperty(ref _isPaused, value))
            {
                PauseResumeText = value ? _resumeLabel : _pauseLabel;
            }
        }
    }

    private string _pauseResumeText = string.Empty;
    public string PauseResumeText
    {
        get => _pauseResumeText;
        private set => SetProperty(ref _pauseResumeText, value);
    }

    private string _pauseLabel = string.Empty;
    private string _resumeLabel = string.Empty;

    public ICommand PauseResumeCommand { get; }

    private string _startWithWindowsLabel = string.Empty;
    public string StartWithWindowsLabel
    {
        get => _startWithWindowsLabel;
        private set => SetProperty(ref _startWithWindowsLabel, value);
    }

    private bool _startWithWindows;
    public bool StartWithWindows
    {
        get => _startWithWindows;
        set
        {
            if (SetProperty(ref _startWithWindows, value))
            {
                _onToggleStartWithWindows(value);
            }
        }
    }

    private readonly Action<bool> _onToggleStartWithWindows;

    private string _settingsText = string.Empty;
    public string SettingsText
    {
        get => _settingsText;
        private set => SetProperty(ref _settingsText, value);
    }

    public ICommand OpenSettingsCommand { get; }

    public WallpaperPreviewViewModel(
        ILocalizationService localization,
        WallpaperContent? content,
        UserPreferences preferences,
        WallpaperApplyStatus applyStatus,
        int initialRemainingSeconds,
        Action onChangeNow,
        Action onOpenSettings,
        Action<bool> onTogglePause,
        Action<bool> onToggleStartWithWindows)
    {
        _localization = localization;
        _preferences = preferences;
        _onToggleStartWithWindows = onToggleStartWithWindows;
        _startWithWindows = preferences.StartWithWindows;

        ChangeNowCommand = new RelayCommand(_ => onChangeNow());
        OpenSettingsCommand = new RelayCommand(_ => onOpenSettings());
        PauseResumeCommand = new RelayCommand(_ =>
        {
            IsPaused = !IsPaused;
            onTogglePause(IsPaused);
        });

        _isPaused = preferences.IsPaused;
        RemainingTimeText = FormatRemaining(initialRemainingSeconds);

        ApplyContent(content, applyStatus);
    }

    /// <summary>
    /// Recalcula todo lo que depende del contenido mostrado y/o del idioma
    /// actual (textos localizados, frase, autor, categorías, estilo, imagen,
    /// estado de rotación) sobre ESTE mismo ViewModel. La usa tanto el
    /// constructor como MainViewModel al volver de la pantalla de
    /// configuración: así la pantalla de vista previa no se reconstruye
    /// entera, solo se actualiza el contenido mostrado.
    /// </summary>
    public void ApplyContent(WallpaperContent? content, WallpaperApplyStatus applyStatus)
    {
        StartWithWindowsLabel = _localization.Get("preview.startWithWindows");
        ChangeNowText = _localization.Get("preview.changeNow");

        HasContent = content is not null;
        NoContentMessage = _localization.Get("preview.noContent");

        StatusText = applyStatus switch
        {
            WallpaperApplyStatus.Applied => _localization.Get("preview.status.applied"),
            WallpaperApplyStatus.ImageMissing => _localization.Get("preview.status.imageMissing"),
            WallpaperApplyStatus.Failed => _localization.Get("preview.status.failed"),
            _ => string.Empty
        };

        ShowRotationControls = _preferences.WallpaperInterval.HasValue
            && _preferences.WallpaperInterval.Value != WallpaperFrequency.Manual;
        ManualModeMessage = _localization.Get("preview.manualMode");
        NextChangeLabel = _localization.Get("preview.nextChange");
        PausedMessage = _localization.Get("preview.paused");

        _pauseLabel = _localization.Get("preview.pause");
        _resumeLabel = _localization.Get("preview.resume");
        PauseResumeText = IsPaused ? _resumeLabel : _pauseLabel;

        if (content is not null)
        {
            Quote = content.Quote;
            QuoteLabel = _localization.Get("preview.quote");
            HasAuthor = !string.IsNullOrWhiteSpace(content.Author);
            AuthorText = HasAuthor ? $"— {content.Author}" : string.Empty;

            CategoriesLabel = _localization.Get("preview.categories");
            CategoriesValue = string.Join(", ", content.Categories.Select(c => _localization.Get(c.LocalizationKey())));

            StyleLabel = _localization.Get("preview.style");
            StyleValue = _localization.Get(content.Style.LocalizationKey());

            PlaceholderColorHex = StylePalette.BackgroundHex(content.Style);
            PlaceholderTextColorHex = StylePalette.TextHex(content.Style);

            var fullPath = SafePaths.ResolveCatalogImage(content.ImagePath);
            HasImage = fullPath is not null && File.Exists(fullPath);
            ImageFullPath = HasImage ? fullPath : null;
        }
        else
        {
            Quote = string.Empty;
            HasAuthor = false;
            AuthorText = string.Empty;
            CategoriesValue = string.Empty;
            StyleValue = string.Empty;
            HasImage = false;
            ImageFullPath = null;
        }

        SettingsText = _localization.Get("preview.settings");
    }

    /// <summary>
    /// Actualiza solo el texto de la cuenta regresiva, llamado por el timer
    /// de MainViewModel una vez por segundo. Deliberadamente no recrea el
    /// ViewModel completo: así no se reinicia la animación de entrada de la
    /// pantalla ni se reconstruyen las listas de opciones cada segundo.
    /// </summary>
    public void UpdateCountdown(int remainingSeconds)
    {
        RemainingTimeText = FormatRemaining(remainingSeconds);
    }

    /// <summary>
    /// Sincroniza el estado de pausa cuando se activa desde el menú de la
    /// bandeja del sistema en vez del botón de esta pantalla.
    /// </summary>
    public void ApplyExternalPauseState(bool isPaused)
    {
        IsPaused = isPaused;
    }

    private static string FormatRemaining(int totalSeconds)
    {
        if (totalSeconds < 0)
        {
            totalSeconds = 0;
        }

        var time = TimeSpan.FromSeconds(totalSeconds);
        return time.TotalHours >= 1
            ? $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}"
            : $"{time.Minutes:00}:{time.Seconds:00}";
    }
}
