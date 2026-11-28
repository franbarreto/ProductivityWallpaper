using System.Windows.Threading;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Models;

namespace ProductivityWallpaper.App.Widgets;

/// <summary>
/// Crea, muestra, oculta y actualiza los widgets de escritorio según las
/// preferencias. Usa UN solo temporizador para todos, alineado al cambio de
/// minuto (o cada segundo solo si el reloj muestra segundos y el modo no es
/// de bajo rendimiento), y lo detiene por completo si no hay ningún widget.
/// </summary>
public sealed class WidgetManager : IDisposable
{
    private readonly UserPreferences _preferences;
    private readonly Action _savePreferences;
    private readonly ClockWidgetViewModel _clock;
    private readonly DayProgressWidgetViewModel _dayProgress;

    private DesktopWidgetWindow? _clockWindow;
    private DesktopWidgetWindow? _dayProgressWindow;
    private DispatcherTimer? _timer;

    public WidgetManager(UserPreferences preferences, ILocalizationService localization, Action savePreferences)
    {
        _preferences = preferences;
        _savePreferences = savePreferences;
        _clock = new ClockWidgetViewModel(localization);
        _dayProgress = new DayProgressWidgetViewModel(localization);
    }

    /// <summary>Sincroniza ventanas y contenido con las preferencias actuales.</summary>
    public void Refresh()
    {
        var low = PerformanceProfile.IsLow;
        var showSeconds = _preferences.ClockShowSeconds && !low;

        foreach (var widget in new WidgetViewModelBase[] { _clock, _dayProgress })
        {
            widget.ApplyAppearance(_preferences.WidgetSize, _preferences.WidgetCardBackground, useTextShadow: !low);
        }

        _clock.Configure(showSeconds, _preferences.Clock24Hour, _preferences.ClockShowDate);

        _clockWindow = Sync(
            _clockWindow, _clock, _preferences.ShowClockWidget, slot: 0,
            _preferences.ClockWidgetLeft, _preferences.ClockWidgetTop,
            (left, top) =>
            {
                _preferences.ClockWidgetLeft = left;
                _preferences.ClockWidgetTop = top;
                _savePreferences();
            });

        _dayProgressWindow = Sync(
            _dayProgressWindow, _dayProgress, _preferences.ShowDayProgressWidget, slot: 1,
            _preferences.DayProgressWidgetLeft, _preferences.DayProgressWidgetTop,
            (left, top) =>
            {
                _preferences.DayProgressWidgetLeft = left;
                _preferences.DayProgressWidgetTop = top;
                _savePreferences();
            });

        UpdateTimer();
    }

    public void Dispose()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Tick -= OnTick;
            _timer = null;
        }

        _clockWindow?.Close();
        _clockWindow = null;
        _dayProgressWindow?.Close();
        _dayProgressWindow = null;
    }

    private static DesktopWidgetWindow? Sync(
        DesktopWidgetWindow? window,
        WidgetViewModelBase viewModel,
        bool enabled,
        int slot,
        double? savedLeft,
        double? savedTop,
        Action<double, double> onMoved)
    {
        if (!enabled)
        {
            window?.Close();
            return null;
        }

        if (window is null)
        {
            window = new DesktopWidgetWindow(viewModel, savedLeft, savedTop, slot);
            window.Moved += onMoved;
            window.Show();
        }

        return window;
    }

    private void UpdateTimer()
    {
        if (_clockWindow is null && _dayProgressWindow is null)
        {
            _timer?.Stop();
            return;
        }

        if (_timer is null)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnTick;
        }

        TickWidgets();
        ScheduleNextTick();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        TickWidgets();
        ScheduleNextTick();
    }

    private void TickWidgets()
    {
        var now = DateTime.Now;

        if (_clockWindow is not null)
        {
            _clock.Tick(now);
        }

        if (_dayProgressWindow is not null)
        {
            _dayProgress.Tick(now);
        }
    }

    private void ScheduleNextTick()
    {
        if (_timer is null)
        {
            return;
        }

        var now = DateTime.Now;
        var everySecond = _clockWindow is not null && _preferences.ClockShowSeconds && !PerformanceProfile.IsLow;

        // Sin segundos, despertar solo al comienzo de cada minuto: la hora
        // nunca se ve atrasada y el proceso duerme el resto del tiempo.
        var milliseconds = everySecond
            ? 1000 - now.Millisecond + 5
            : (60 - now.Second) * 1000 - now.Millisecond + 50;

        _timer.Stop();
        _timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
        _timer.Start();
    }
}
