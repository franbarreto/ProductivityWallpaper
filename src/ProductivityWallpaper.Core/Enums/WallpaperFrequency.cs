namespace ProductivityWallpaper.Core.Enums;

public enum WallpaperFrequency
{
    Minutes15,
    Minutes30,
    Hour1,
    Hours3,
    Day1,
    Manual
}

public static class WallpaperFrequencyExtensions
{
    /// <summary>
    /// Convierte la frecuencia a segundos. Devuelve null cuando es Manual,
    /// ya que en ese caso no debe producirse ningún cambio automático.
    /// </summary>
    public static int? ToSeconds(this WallpaperFrequency frequency) => frequency switch
    {
        WallpaperFrequency.Minutes15 => 900,
        WallpaperFrequency.Minutes30 => 1800,
        WallpaperFrequency.Hour1 => 3600,
        WallpaperFrequency.Hours3 => 10800,
        WallpaperFrequency.Day1 => 86400,
        WallpaperFrequency.Manual => null,
        _ => null
    };
}
