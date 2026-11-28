using System.IO;
using ProductivityWallpaper.Infrastructure.Preferences;

namespace ProductivityWallpaper.App.Services;

/// <summary>
/// Registro de errores en un archivo local (error.log, en la carpeta de datos
/// de la app). Los detalles técnicos (rutas, pila de llamadas) van al archivo
/// y no a la pantalla, para no exponerlos en una captura o al compartir la
/// pantalla. El archivo se recorta cuando pasa de 256 KB, así no crece sin límite.
/// </summary>
public static class ErrorLog
{
    private const long MaxBytes = 256 * 1024;
    private static readonly object Gate = new();

    public static string FilePath => Path.Combine(AppDataLocation.Folder, "error.log");

    public static void Write(Exception exception)
    {
        try
        {
            lock (Gate)
            {
                var info = new FileInfo(FilePath);
                if (info.Exists && info.Length > MaxBytes)
                {
                    info.Delete();
                }

                File.AppendAllText(
                    FilePath,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {exception}{Environment.NewLine}{Environment.NewLine}");
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Si ni siquiera se puede escribir el log, no hay nada más que hacer.
        }
    }
}
