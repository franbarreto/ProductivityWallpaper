namespace ProductivityWallpaper.Core.Interfaces;

/// <summary>
/// Registra o quita el inicio automático de la app con Windows (Fase 6).
/// La implementación concreta (registro de Windows) vive en Infrastructure;
/// Core no sabe nada de Win32 ni del registro.
/// </summary>
public interface IStartupService
{
    /// <summary>
    /// Activa el inicio automático con Windows. Devuelve false si no se
    /// pudo escribir el registro, sin lanzar excepción.
    /// </summary>
    bool Enable();

    /// <summary>
    /// Desactiva el inicio automático con Windows. Devuelve false si no se
    /// pudo quitar la entrada del registro, sin lanzar excepción.
    /// </summary>
    bool Disable();

    /// <summary>
    /// Consulta el estado real en el registro de Windows (no la preferencia
    /// guardada), para poder corregir la UI si alguien borró la entrada a mano.
    /// </summary>
    bool IsEnabled();
}
