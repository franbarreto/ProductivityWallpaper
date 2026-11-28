using System.Runtime.Versioning;
using Microsoft.Win32;
using ProductivityWallpaper.Core.Interfaces;

namespace ProductivityWallpaper.Infrastructure.Startup;

/// <summary>
/// Inicio automático con Windows (Fase 6) vía el registro de usuario
/// estándar (HKCU\...\Run), el mismo mecanismo que usan la mayoría de las
/// apps de escritorio livianas. No requiere permisos de administrador ni
/// una tarea programada: alcanza con una entrada en esa clave.
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsStartupService : IStartupService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ProductivityWallpaper";

    public bool Enable()
    {
        var command = GetStartupCommand();
        if (command is null)
        {
            return false;
        }

        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
            key?.SetValue(ValueName, command, RegistryValueKind.String);
            return key is not null;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException)
        {
            return false;
        }
    }

    public bool Disable()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
            if (key?.GetValue(ValueName) is not null)
            {
                key.DeleteValue(ValueName, throwOnMissingValue: false);
            }
            return true;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException)
        {
            return false;
        }
    }

    public bool IsEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            return key?.GetValue(ValueName) is not null;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException)
        {
            return false;
        }
    }

    /// <summary>
    /// Comando que se registra para iniciar la app con Windows:
    ///  - el .exe publicado, entre comillas (así una ruta con espacios no se
    ///    interpreta como varios argumentos ni permite ejecutar otro programa);
    ///  - o, si la app se está ejecutando a través de "dotnet" (versión
    ///    portable en una PC que bloquea el .exe), el lanzador Iniciar.bat que
    ///    viaja en la misma carpeta, que ya sabe probar las dos formas.
    /// Con "dotnet run" no hay ninguno de los dos y Enable() devuelve false.
    /// </summary>
    private static string? GetStartupCommand()
    {
        var path = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(path) &&
            path.EndsWith("ProductivityWallpaper.exe", StringComparison.OrdinalIgnoreCase) &&
            !path.Contains('"'))
        {
            return $"\"{path}\"";
        }

        var launcher = Path.Combine(AppContext.BaseDirectory, "Iniciar.bat");
        if (File.Exists(launcher) && !launcher.Contains('"'))
        {
            return $"\"{launcher}\"";
        }

        return null;
    }
}
