using System.IO;

namespace ProductivityWallpaper.App.Services;

/// <summary>
/// Resuelve la ruta de una imagen del catálogo asegurándose de que quede
/// dentro de la carpeta Content de la app. Un catálogo con una ruta como
/// "..\..\otra-cosa" o una ruta absoluta no puede hacer que la app lea o
/// aplique como fondo un archivo de cualquier otra parte del disco.
/// </summary>
public static class SafePaths
{
    public static string? ResolveCatalogImage(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath))
        {
            return null;
        }

        try
        {
            var baseDirectory = Path.GetFullPath(AppContext.BaseDirectory);
            var allowedRoot = Path.Combine(baseDirectory, "Content") + Path.DirectorySeparatorChar;
            var fullPath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));

            return fullPath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase) ? fullPath : null;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return null;
        }
    }
}
