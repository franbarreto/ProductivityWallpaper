using System.Windows;
using System.Windows.Threading;
using ProductivityWallpaper.App.Services;

namespace ProductivityWallpaper.App;

public partial class App : Application
{
    private SingleInstanceGuard? _singleInstance;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Una sola instancia: si ya hay una abierta, se le avisa y esta se cierra
        // sin crear ninguna ventana.
        _singleInstance = new SingleInstanceGuard();
        if (!_singleInstance.IsFirstInstance)
        {
            Shutdown();
            return;
        }

        _singleInstance.ListenForShowRequests(() =>
            Dispatcher.BeginInvoke(new Action(() => (MainWindow as MainWindow)?.BringToFront())));

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                ErrorLog.Write(exception);
            }
        };
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            ErrorLog.Write(args.Exception);
            args.SetObserved();
        };

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _singleInstance?.Dispose();
        base.OnExit(e);
    }

    /// <summary>
    /// Red de seguridad: si algo no previsto revienta en el hilo de UI, el
    /// detalle técnico se guarda en error.log y se muestra un mensaje simple
    /// (sin rutas ni pila de llamadas) en vez de que la app desaparezca en
    /// silencio.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ErrorLog.Write(e.Exception);
        MessageBox.Show(
            $"Productivity Wallpaper encontró un error inesperado y necesita cerrarse.\n\nSe guardó un detalle en:\n{ErrorLog.FilePath}",
            "Error inesperado",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
        Shutdown();
    }
}
