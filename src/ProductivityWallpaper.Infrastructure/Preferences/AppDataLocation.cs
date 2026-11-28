namespace ProductivityWallpaper.Infrastructure.Preferences;

/// <summary>
/// Decide dónde guarda la app sus datos (preferencias y registro de errores).
///
/// Modo portable: si junto al ejecutable existe un archivo vacío llamado
/// "portable.flag", todo se guarda en la subcarpeta "Data" de la propia
/// carpeta de la app. Así se puede copiar la carpeta a un pendrive u otra PC
/// y no deja nada en el perfil del usuario. Si esa carpeta no se puede
/// escribir (por ejemplo, está en Program Files), se vuelve en silencio a
/// %LOCALAPPDATA% en vez de fallar.
/// </summary>
public static class AppDataLocation
{
    private const string PortableFlagFile = "portable.flag";

    public static bool IsPortable { get; }

    public static string Folder { get; }

    static AppDataLocation()
    {
        var portableFolder = Path.Combine(AppContext.BaseDirectory, "Data");
        var wantsPortable = File.Exists(Path.Combine(AppContext.BaseDirectory, PortableFlagFile));

        if (wantsPortable && TryPrepareFolder(portableFolder))
        {
            IsPortable = true;
            Folder = portableFolder;
            return;
        }

        var roaming = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ProductivityWallpaper");
        TryPrepareFolder(roaming);
        Folder = roaming;
    }

    private static bool TryPrepareFolder(string folder)
    {
        try
        {
            Directory.CreateDirectory(folder);

            // Comprobación real de escritura: crear la carpeta no alcanza si
            // los permisos son de solo lectura.
            var probe = Path.Combine(folder, ".write-test");
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            return false;
        }
    }
}
