namespace ProductivityWallpaper.App.Services;

/// <summary>
/// Garantiza una sola instancia de la app por sesión de Windows. Dos
/// instancias cambiarían el wallpaper a la vez y se pisarían las
/// preferencias. Si el usuario abre la app de nuevo, la segunda instancia le
/// avisa a la primera (para que se muestre) y se cierra.
/// </summary>
public sealed class SingleInstanceGuard : IDisposable
{
    private const string MutexName = @"Local\ProductivityWallpaper.SingleInstance";
    private const string ShowEventName = @"Local\ProductivityWallpaper.ShowWindow";

    private readonly Mutex _mutex;
    private readonly EventWaitHandle _showEvent;
    private RegisteredWaitHandle? _registration;

    public bool IsFirstInstance { get; }

    public SingleInstanceGuard()
    {
        _mutex = new Mutex(initiallyOwned: true, MutexName, out var createdNew);
        _showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowEventName);
        IsFirstInstance = createdNew;

        if (!createdNew)
        {
            // Ya hay otra instancia corriendo: pedirle que se muestre.
            _showEvent.Set();
        }
    }

    /// <summary>La primera instancia llama a esto para enterarse cuando se intente abrir otra.</summary>
    public void ListenForShowRequests(Action onShowRequested)
    {
        _registration = ThreadPool.RegisterWaitForSingleObject(
            _showEvent,
            (_, _) => onShowRequested(),
            null,
            Timeout.Infinite,
            executeOnlyOnce: false);
    }

    public void Dispose()
    {
        _registration?.Unregister(null);
        _showEvent.Dispose();

        if (IsFirstInstance)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch (ApplicationException)
            {
                // El mutex lo creó otro hilo: al terminar el proceso se libera igual.
            }
        }

        _mutex.Dispose();
    }
}
