using ProductivityWallpaper.App.Services;

namespace ProductivityWallpaper.App.Widgets;

/// <summary>Cuánto del día (de 00:00 a 24:00) ya pasó, con barra y tiempo restante.</summary>
public sealed class DayProgressWidgetViewModel : WidgetViewModelBase
{
    private readonly ILocalizationService _localization;

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    private string _percentText = string.Empty;
    public string PercentText
    {
        get => _percentText;
        private set => SetProperty(ref _percentText, value);
    }

    private double _progress;
    public double Progress
    {
        get => _progress;
        private set => SetProperty(ref _progress, value);
    }

    private string _remainingText = string.Empty;
    public string RemainingText
    {
        get => _remainingText;
        private set => SetProperty(ref _remainingText, value);
    }

    private double _percentFontSize = 44;
    public double PercentFontSize
    {
        get => _percentFontSize;
        private set => SetProperty(ref _percentFontSize, value);
    }

    private double _labelFontSize = 14;
    public double LabelFontSize
    {
        get => _labelFontSize;
        private set => SetProperty(ref _labelFontSize, value);
    }

    private double _barWidth = 240;
    public double BarWidth
    {
        get => _barWidth;
        private set => SetProperty(ref _barWidth, value);
    }

    private double _barHeight = 8;
    public double BarHeight
    {
        get => _barHeight;
        private set => SetProperty(ref _barHeight, value);
    }

    public DayProgressWidgetViewModel(ILocalizationService localization)
    {
        _localization = localization;
    }

    public override void Tick(DateTime now)
    {
        var start = now.Date;
        var end = start.AddDays(1);
        var fraction = Math.Clamp((now - start).TotalSeconds / (end - start).TotalSeconds, 0, 1);

        Progress = fraction * 100;
        PercentText = $"{(int)Math.Floor(fraction * 100)}%";

        var remaining = end - now;
        Title = _localization.Get("widget.dayProgress.title");
        RemainingText = string.Format(
            _localization.Get("widget.dayProgress.remaining"),
            (int)remaining.TotalHours,
            remaining.Minutes);
    }

    protected override void OnAppearanceChanged()
    {
        PercentFontSize = 44 * Scale;
        LabelFontSize = 14 * Scale;
        BarWidth = 240 * Scale;
        BarHeight = Math.Max(5, 8 * Scale);
    }
}
