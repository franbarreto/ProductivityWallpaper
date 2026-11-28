using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Localization;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;
using ProductivityWallpaper.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

/// <summary>
/// Pantalla única de configuración, estilo "Ajustes" de iOS: agrupa en
/// secciones todo lo que antes requería pasar paso a paso por el wizard de
/// onboarding (idioma, intereses, estilo, frecuencia), más el inicio con
/// Windows. Los cambios se aplican de inmediato sobre el mismo
/// UserPreferences "draft" que usa MainViewModel; "Listo" confirma y hace
/// que se recalcule/aplique el wallpaper con las preferencias nuevas.
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    private readonly ILocalizationService _localization;
    private readonly UserPreferences _draft;
    private readonly Action<bool> _onToggleStartWithWindows;
    private readonly Action _onPerformanceChanged;
    private readonly Action _onWidgetsChanged;

    /// <summary>Textos de las secciones Rendimiento y Widgets.</summary>
    public SettingsTexts Texts { get; } = new();

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    private string _doneText = string.Empty;
    public string DoneText
    {
        get => _doneText;
        private set => SetProperty(ref _doneText, value);
    }

    private string _languageHeader = string.Empty;
    public string LanguageHeader
    {
        get => _languageHeader;
        private set => SetProperty(ref _languageHeader, value);
    }

    private string _interestsHeader = string.Empty;
    public string InterestsHeader
    {
        get => _interestsHeader;
        private set => SetProperty(ref _interestsHeader, value);
    }

    private string _styleHeader = string.Empty;
    public string StyleHeader
    {
        get => _styleHeader;
        private set => SetProperty(ref _styleHeader, value);
    }

    private string _frequencyHeader = string.Empty;
    public string FrequencyHeader
    {
        get => _frequencyHeader;
        private set => SetProperty(ref _frequencyHeader, value);
    }

    private string _systemHeader = string.Empty;
    public string SystemHeader
    {
        get => _systemHeader;
        private set => SetProperty(ref _systemHeader, value);
    }

    private string _startWithWindowsLabel = string.Empty;
    public string StartWithWindowsLabel
    {
        get => _startWithWindowsLabel;
        private set => SetProperty(ref _startWithWindowsLabel, value);
    }

    private string _languageEsText = string.Empty;
    public string LanguageEsText
    {
        get => _languageEsText;
        private set => SetProperty(ref _languageEsText, value);
    }

    private string _languageEnText = string.Empty;
    public string LanguageEnText
    {
        get => _languageEnText;
        private set => SetProperty(ref _languageEnText, value);
    }

    public ObservableCollection<InterestOption> InterestOptions { get; } = new();
    public ObservableCollection<StyleOption> StyleOptions { get; } = new();
    public ObservableCollection<FrequencyOption> FrequencyOptions { get; } = new();

    private bool _isEsSelected;
    public bool IsEsSelected
    {
        get => _isEsSelected;
        set
        {
            if (SetProperty(ref _isEsSelected, value) && value)
            {
                SelectLanguage(AppLanguage.Es);
            }
        }
    }

    private bool _isEnSelected;
    public bool IsEnSelected
    {
        get => _isEnSelected;
        set
        {
            if (SetProperty(ref _isEnSelected, value) && value)
            {
                SelectLanguage(AppLanguage.En);
            }
        }
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

    // --- Rendimiento ---

    private bool _isPerformanceLow;
    public bool IsPerformanceLow
    {
        get => _isPerformanceLow;
        set
        {
            if (SetProperty(ref _isPerformanceLow, value) && value)
            {
                SelectPerformance(PerformanceMode.Low);
            }
        }
    }

    private bool _isPerformanceHigh;
    public bool IsPerformanceHigh
    {
        get => _isPerformanceHigh;
        set
        {
            if (SetProperty(ref _isPerformanceHigh, value) && value)
            {
                SelectPerformance(PerformanceMode.High);
            }
        }
    }

    /// <summary>Mostrar segundos no está disponible en bajo rendimiento.</summary>
    public bool CanShowSeconds => !_isPerformanceLow;

    // --- Widgets ---

    private bool _showClockWidget;
    public bool ShowClockWidget
    {
        get => _showClockWidget;
        set
        {
            if (SetProperty(ref _showClockWidget, value))
            {
                _draft.ShowClockWidget = value;
                _onWidgetsChanged();
            }
        }
    }

    private bool _clockShowSeconds;
    public bool ClockShowSeconds
    {
        get => _clockShowSeconds;
        set
        {
            if (SetProperty(ref _clockShowSeconds, value))
            {
                _draft.ClockShowSeconds = value;
                _onWidgetsChanged();
            }
        }
    }

    private bool _clock24Hour;
    public bool Clock24Hour
    {
        get => _clock24Hour;
        set
        {
            if (SetProperty(ref _clock24Hour, value))
            {
                _draft.Clock24Hour = value;
                _onWidgetsChanged();
            }
        }
    }

    private bool _clockShowDate;
    public bool ClockShowDate
    {
        get => _clockShowDate;
        set
        {
            if (SetProperty(ref _clockShowDate, value))
            {
                _draft.ClockShowDate = value;
                _onWidgetsChanged();
            }
        }
    }

    private bool _showDayProgressWidget;
    public bool ShowDayProgressWidget
    {
        get => _showDayProgressWidget;
        set
        {
            if (SetProperty(ref _showDayProgressWidget, value))
            {
                _draft.ShowDayProgressWidget = value;
                _onWidgetsChanged();
            }
        }
    }

    private bool _widgetCardBackground;
    public bool WidgetCardBackground
    {
        get => _widgetCardBackground;
        set
        {
            if (SetProperty(ref _widgetCardBackground, value))
            {
                _draft.WidgetCardBackground = value;
                _onWidgetsChanged();
            }
        }
    }

    private bool _isWidgetSizeSmall;
    public bool IsWidgetSizeSmall
    {
        get => _isWidgetSizeSmall;
        set
        {
            if (SetProperty(ref _isWidgetSizeSmall, value) && value)
            {
                SelectWidgetSize(WidgetSize.Small);
            }
        }
    }

    private bool _isWidgetSizeMedium;
    public bool IsWidgetSizeMedium
    {
        get => _isWidgetSizeMedium;
        set
        {
            if (SetProperty(ref _isWidgetSizeMedium, value) && value)
            {
                SelectWidgetSize(WidgetSize.Medium);
            }
        }
    }

    private bool _isWidgetSizeLarge;
    public bool IsWidgetSizeLarge
    {
        get => _isWidgetSizeLarge;
        set
        {
            if (SetProperty(ref _isWidgetSizeLarge, value) && value)
            {
                SelectWidgetSize(WidgetSize.Large);
            }
        }
    }

    public ICommand DoneCommand { get; }

    public SettingsViewModel(
        ILocalizationService localization,
        UserPreferences draft,
        Action onDone,
        Action<bool> onToggleStartWithWindows,
        Action onPerformanceChanged,
        Action onWidgetsChanged)
    {
        _localization = localization;
        _draft = draft;
        _onToggleStartWithWindows = onToggleStartWithWindows;
        _onPerformanceChanged = onPerformanceChanged;
        _onWidgetsChanged = onWidgetsChanged;

        _isEsSelected = draft.Language == AppLanguage.Es;
        _isEnSelected = draft.Language == AppLanguage.En;
        _startWithWindows = draft.StartWithWindows;

        _isPerformanceLow = draft.Performance == PerformanceMode.Low;
        _isPerformanceHigh = draft.Performance == PerformanceMode.High;
        _showClockWidget = draft.ShowClockWidget;
        _clockShowSeconds = draft.ClockShowSeconds;
        _clock24Hour = draft.Clock24Hour;
        _clockShowDate = draft.ClockShowDate;
        _showDayProgressWidget = draft.ShowDayProgressWidget;
        _widgetCardBackground = draft.WidgetCardBackground;
        _isWidgetSizeSmall = draft.WidgetSize == WidgetSize.Small;
        _isWidgetSizeMedium = draft.WidgetSize == WidgetSize.Medium;
        _isWidgetSizeLarge = draft.WidgetSize == WidgetSize.Large;

        DoneCommand = new RelayCommand(_ => onDone());

        RebuildOptions();
        RefreshLocalizedText();
    }

    /// <summary>
    /// Cambiar de idioma acá reconstruye el CONTENIDO de las colecciones de
    /// opciones (Clear + Add sobre las mismas instancias de
    /// ObservableCollection ya enlazadas en el XAML), nunca el ViewModel ni
    /// la pantalla entera: así no se repite el bug de re-entrancia que
    /// causaba reconstruir toda la vista desde el propio evento de un
    /// CheckBox (ver WallpaperPreviewViewModel.ApplyContent). El selector de
    /// idioma es un control aparte de las listas de intereses/estilo/
    /// frecuencia, así que no hay conflicto con su propio evento de click.
    /// </summary>
    private void SelectLanguage(AppLanguage language)
    {
        _draft.Language = language;
        _localization.CurrentLanguage = language;
        IsEsSelected = language == AppLanguage.Es;
        IsEnSelected = language == AppLanguage.En;
        RebuildOptions();
        RefreshLocalizedText();
        _onWidgetsChanged();
    }

    private void SelectPerformance(PerformanceMode mode)
    {
        _draft.Performance = mode;
        IsPerformanceLow = mode == PerformanceMode.Low;
        IsPerformanceHigh = mode == PerformanceMode.High;
        OnPropertyChanged(nameof(CanShowSeconds));
        Texts.Refresh(_localization, mode);
        _onPerformanceChanged();
    }

    private void SelectWidgetSize(WidgetSize size)
    {
        _draft.WidgetSize = size;
        IsWidgetSizeSmall = size == WidgetSize.Small;
        IsWidgetSizeMedium = size == WidgetSize.Medium;
        IsWidgetSizeLarge = size == WidgetSize.Large;
        _onWidgetsChanged();
    }

    private void RebuildOptions()
    {
        InterestOptions.Clear();
        foreach (var category in Enum.GetValues<InterestCategory>())
        {
            var option = new InterestOption
            {
                Category = category,
                DisplayName = _localization.Get(category.LocalizationKey()),
                IsSelected = _draft.SelectedCategories.Contains(category)
            };
            option.PropertyChanged += (_, _) =>
            {
                _draft.SelectedCategories = InterestOptions
                    .Where(o => o.IsSelected)
                    .Select(o => o.Category)
                    .ToList();
            };
            InterestOptions.Add(option);
        }

        StyleOptions.Clear();
        foreach (var style in Enum.GetValues<VisualStyle>())
        {
            var option = new StyleOption
            {
                Style = style,
                DisplayName = _localization.Get(style.LocalizationKey()),
                IsSelected = style == _draft.SelectedStyle
            };
            option.PropertyChanged += (_, _) =>
            {
                if (option.IsSelected)
                {
                    _draft.SelectedStyle = option.Style;
                }
            };
            StyleOptions.Add(option);
        }

        FrequencyOptions.Clear();
        foreach (var frequency in Enum.GetValues<WallpaperFrequency>())
        {
            var option = new FrequencyOption
            {
                Frequency = frequency,
                DisplayName = _localization.Get(frequency.LocalizationKey()),
                IsSelected = frequency == _draft.WallpaperInterval
            };
            option.PropertyChanged += (_, _) =>
            {
                if (option.IsSelected)
                {
                    _draft.WallpaperInterval = option.Frequency;
                    _draft.AutoChange = option.Frequency != WallpaperFrequency.Manual;
                }
            };
            FrequencyOptions.Add(option);
        }
    }

    private void RefreshLocalizedText()
    {
        Title = _localization.Get("settings.title");
        DoneText = _localization.Get("settings.done");
        LanguageHeader = _localization.Get("settings.section.language");
        InterestsHeader = _localization.Get("settings.section.interests");
        StyleHeader = _localization.Get("settings.section.style");
        FrequencyHeader = _localization.Get("settings.section.frequency");
        SystemHeader = _localization.Get("settings.section.system");
        StartWithWindowsLabel = _localization.Get("preview.startWithWindows");
        LanguageEsText = _localization.Get("language.es");
        LanguageEnText = _localization.Get("language.en");
        Texts.Refresh(_localization, _draft.Performance);
    }
}
