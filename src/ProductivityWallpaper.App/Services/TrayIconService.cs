using System.IO;
using WinForms = System.Windows.Forms;

namespace ProductivityWallpaper.App.Services;

/// <summary>
/// Ícono de la bandeja del sistema que permite que la app siga corriendo en
/// segundo plano al cerrar la ventana (como Wallpaper Engine). WPF no tiene
/// un control nativo para esto, así que se usa System.Windows.Forms.NotifyIcon
/// (referencia de framework del SDK, no un paquete NuGet).
///
/// El texto del menú se recalcula cada vez que se abre (evento Opening) en
/// vez de guardarse una sola vez, así siempre refleja el idioma actual y el
/// estado de pausa actual sin que este servicio necesite saber nada de
/// MainViewModel más allá de los Func/Action que recibe.
/// </summary>
public sealed class TrayIconService : IDisposable
{
    private readonly WinForms.NotifyIcon _notifyIcon;
    private readonly WinForms.ToolStripMenuItem _showItem;
    private readonly WinForms.ToolStripMenuItem _changeNowItem;
    private readonly WinForms.ToolStripMenuItem _pauseResumeItem;
    private readonly WinForms.ToolStripMenuItem _exitItem;

    private readonly ILocalizationService _localization;
    private readonly Func<bool> _hasRotationControls;
    private readonly Func<bool> _isPaused;

    public TrayIconService(
        string iconFilePath,
        ILocalizationService localization,
        Func<bool> hasRotationControls,
        Func<bool> isPaused,
        Action onShowRequested,
        Action onChangeNow,
        Action onTogglePause,
        Action onExit)
    {
        _localization = localization;
        _hasRotationControls = hasRotationControls;
        _isPaused = isPaused;

        _showItem = new WinForms.ToolStripMenuItem();
        _changeNowItem = new WinForms.ToolStripMenuItem();
        _pauseResumeItem = new WinForms.ToolStripMenuItem();
        _exitItem = new WinForms.ToolStripMenuItem();

        _showItem.Click += (_, _) => onShowRequested();
        _changeNowItem.Click += (_, _) => onChangeNow();
        _pauseResumeItem.Click += (_, _) => onTogglePause();
        _exitItem.Click += (_, _) => onExit();

        var contextMenu = new WinForms.ContextMenuStrip();
        contextMenu.Items.Add(_showItem);
        contextMenu.Items.Add(new WinForms.ToolStripSeparator());
        contextMenu.Items.Add(_changeNowItem);
        contextMenu.Items.Add(_pauseResumeItem);
        contextMenu.Items.Add(new WinForms.ToolStripSeparator());
        contextMenu.Items.Add(_exitItem);
        contextMenu.Opening += (_, _) => RefreshMenuText();

        _notifyIcon = new WinForms.NotifyIcon
        {
            Icon = LoadIconOrFallback(iconFilePath),
            Text = "Productivity Wallpaper",
            ContextMenuStrip = contextMenu,
            Visible = false
        };
        _notifyIcon.DoubleClick += (_, _) => onShowRequested();
    }

    private void RefreshMenuText()
    {
        _showItem.Text = _localization.Get("tray.show");
        _changeNowItem.Text = _localization.Get("tray.changeNow");
        _exitItem.Text = _localization.Get("tray.exit");

        var hasControls = _hasRotationControls();
        _pauseResumeItem.Visible = hasControls;
        if (hasControls)
        {
            _pauseResumeItem.Text = _isPaused() ? _localization.Get("tray.resume") : _localization.Get("tray.pause");
        }
    }

    public void Show() => _notifyIcon.Visible = true;

    public void ShowBalloonTip(string title, string message)
    {
        _notifyIcon.ShowBalloonTip(4000, title, message, WinForms.ToolTipIcon.Info);
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }

    private static System.Drawing.Icon LoadIconOrFallback(string iconFilePath)
    {
        try
        {
            return new System.Drawing.Icon(iconFilePath);
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or System.Runtime.InteropServices.ExternalException)
        {
            // Si por algún motivo el .ico no está en el directorio de salida
            // (un despliegue incompleto, por ejemplo), la bandeja sigue
            // funcionando con el ícono genérico de la aplicación en vez de
            // romper el arranque entero.
            return System.Drawing.SystemIcons.Application;
        }
    }
}
