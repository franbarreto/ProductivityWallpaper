using System.Globalization;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.App.Widgets;

/// <summary>Reloj de escritorio: hora grande y, opcionalmente, la fecha debajo.</summary>
public sealed class ClockWidgetViewModel : WidgetViewModelBase
{
    private readonly ILocalizationService _localization;

    private bool _showSeconds;
    private bool _use24Hour = true;

    private string _timeText = string.Empty;
    public string TimeText
    {
        get => _timeText;
        private set => SetProperty(ref _timeText, value);
    }

    private string _dateText = string.Empty;
    public string DateText
    {
        get => _dateText;
        private set => SetProperty(ref _dateText, value);
    }

    private bool _hasDate = true;
    public bool HasDate
    {
        get => _hasDate;
        private set => SetProperty(ref _hasDate, value);
    }

    private double _timeFontSize = 64;
    public double TimeFontSize
    {
        get => _timeFontSize;
        private set => SetProperty(ref _timeFontSize, value);
    }

    private double _dateFontSize = 18;
    public double DateFontSize
    {
        get => _dateFontSize;
        private set => SetProperty(ref _dateFontSize, value);
    }

    public ClockWidgetViewModel(ILocalizationService localization)
    {
        _localization = localization;
    }

    public void Configure(bool showSeconds, bool use24Hour, bool showDate)
    {
        _showSeconds = showSeconds;
        _use24Hour = use24Hour;
        HasDate = showDate;
    }

    public override void Tick(DateTime now)
    {
        var isSpanish = _localization.CurrentLanguage == AppLanguage.Es;
        var culture = CultureInfo.GetCultureInfo(isSpanish ? "es-ES" : "en-US");

        var timeFormat = _use24Hour
            ? (_showSeconds ? "HH:mm:ss" : "HH:mm")
            : (_showSeconds ? "h:mm:ss tt" : "h:mm tt");
        TimeText = now.ToString(timeFormat, culture);

        var dateFormat = isSpanish ? "dddd, d 'de' MMMM" : "dddd, MMMM d";
        var date = now.ToString(dateFormat, culture);
        DateText = date.Length > 0 ? char.ToUpper(date[0], culture) + date[1..] : date;
    }

    protected override void OnAppearanceChanged()
    {
        TimeFontSize = 64 * Scale;
        DateFontSize = 18 * Scale;
    }
}
