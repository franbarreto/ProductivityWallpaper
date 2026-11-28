using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ProductivityWallpaper.Core.Interfaces;

namespace ProductivityWallpaper.Infrastructure.Wallpaper;

/// <summary>
/// Cambia el fondo de escritorio real de Windows usando SystemParametersInfo
/// (user32.dll), el mecanismo nativo y oficial para esto. Soporta JPG/PNG/BMP
/// directamente desde Windows Vista en adelante.
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsWallpaperService : IWallpaperService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".bmp" };

    private const int SpiSetDeskWallpaper = 0x0014;
    private const int SpifUpdateIniFile = 0x01;
    private const int SpifSendWinIniChange = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public bool ApplyWallpaper(string absoluteImagePath)
    {
        if (string.IsNullOrWhiteSpace(absoluteImagePath) || !File.Exists(absoluteImagePath))
        {
            return false;
        }

        // Solo formatos de imagen: nunca se le pasa a Windows otro tipo de archivo.
        var extension = Path.GetExtension(absoluteImagePath);
        if (!AllowedExtensions.Contains(extension))
        {
            return false;
        }

        return SystemParametersInfo(
            SpiSetDeskWallpaper,
            0,
            absoluteImagePath,
            SpifUpdateIniFile | SpifSendWinIniChange);
    }
}
