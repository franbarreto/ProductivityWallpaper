using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.App.ViewModels;
using ProductivityWallpaper.Core.Interfaces;
using ProductivityWallpaper.Core.Services;
using ProductivityWallpaper.Infrastructure.Preferences;
using ProductivityWallpaper.Infrastructure.Repositories;
using ProductivityWallpaper.Infrastructure.Startup;
using ProductivityWallpaper.Infrastructure.Wallpaper;

namespace ProductivityWallpaper.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;
    private readonly ILocalizationService _localization;
    private TrayIconService? _trayIconService;
    private bool _isExiting;
    private bool _hasShownTrayHint;

    public MainWindow()
    {
        InitializeComponent();

        // Composition root manual: la app es chica, no se justifica un
        // contenedor de inyección de dependencias todavía.
        ILocalizationService localizationService = new LocalizationService();
        IPreferencesStore preferencesStore = new JsonPreferencesStore();
        IWallpaperRepository wallpaperRepository = new JsonWallpaperRepository();
        IWallpaperSelector wallpaperSelector = new WallpaperSelector(wallpaperRepository);
        IWallpaperService wallpaperService = new WindowsWallpaperService();
        IStartupService startupService = new WindowsStartupService();

        _localization = localizationService;
        _mainViewModel = new MainViewModel(localizationService, preferencesStore, wallpaperSelector, wallpaperService, startupService);
        DataContext = _mainViewModel;

        Loaded += OnLoaded;
        Closing += OnClosing;
        StateChanged += OnStateChanged;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");
        TrySetWindowIcon(iconPath);

        _trayIconService = new TrayIconService(
            iconPath,
            _localization,
            hasRotationControls: () => _mainViewModel.HasRotationControls,
            isPaused: () => _mainViewModel.IsRotationPaused,
            onShowRequested: RestoreFromTray,
            onChangeNow: () => _mainViewModel.TriggerChangeNow(),
            onTogglePause: () => _mainViewModel.ToggleRotationFromTray(),
            onExit: ExitApplication);

        _trayIconService.Show();
    }

    /// <summary>
    /// Igual que Wallpaper Engine: cerrar la ventana (la X) no termina el
    /// proceso, lo manda a la bandeja del sistema para que siga cambiando el
    /// wallpaper en segundo plano. Solo "Salir" desde el menú de la bandeja
    /// cierra la app de verdad.
    /// </summary>
    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_isExiting)
        {
            _trayIconService?.Dispose();
            return;
        }

        e.Cancel = true;
        Hide();

        if (!_hasShownTrayHint)
        {
            _hasShownTrayHint = true;
            _trayIconService?.ShowBalloonTip(
                _localization.Get("tray.minimizedTitle"),
                _localization.Get("tray.minimizedMessage"));
        }
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
        }
    }

    private void RestoreFromTray()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
        PlayRestoreAnimation();
    }

    /// <summary>Lo usa la segunda instancia de la app (ver SingleInstanceGuard) para mostrar esta ventana.</summary>
    public void BringToFront() => RestoreFromTray();

    /// <summary>
    /// Aparición suave al volver desde la bandeja (se omite en modo de bajo
    /// rendimiento). Se anima el contenido y no la Window: Window.Opacity solo
    /// funciona con AllowsTransparency, que esta ventana no usa.
    /// </summary>
    private void PlayRestoreAnimation()
    {
        if (Content is not UIElement root || !PerformanceProfile.AnimationsEnabled)
        {
            return;
        }

        root.BeginAnimation(OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220))
        {
            EasingFunction = new System.Windows.Media.Animation.QuadraticEase()
        });
    }

    /// <summary>
    /// El ícono de la ventana se carga desde el .ico real en disco (igual
    /// que el de la bandeja), no desde "Icon=" en el XAML: ese atributo
    /// espera un recurso embebido en el ensamblado (pack URI), y el .ico
    /// solo viaja copiado como archivo suelto junto al .exe. Si se intenta
    /// como recurso embebido sin estarlo, falla al construir la ventana
    /// antes de mostrar nada. Cualquier error acá se ignora: la app sigue
    /// funcionando perfectamente sin ícono propio en la barra de título.
    /// </summary>
    private void TrySetWindowIcon(string iconPath)
    {
        try
        {
            if (System.IO.File.Exists(iconPath))
            {
                Icon = BitmapFrame.Create(new Uri(iconPath, UriKind.Absolute));
            }
        }
        catch (Exception ex) when (ex is NotSupportedException or System.IO.IOException or UriFormatException)
        {
            // Sin ícono propio, pero la app sigue andando.
        }
    }

    private void ExitApplication()
    {
        // _isExiting = true hace que OnClosing esta vez deje seguir el cierre
        // real en lugar de ocultar la ventana. Al cerrarse la última ventana,
        // ShutdownMode=OnMainWindowClose (ver App.xaml) termina el proceso; los widgets de escritorio ya se cerraron arriba.
        _isExiting = true;
        _mainViewModel.Shutdown();
        Close();
    }
}
