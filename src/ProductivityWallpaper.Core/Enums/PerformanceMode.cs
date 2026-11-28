namespace ProductivityWallpaper.Core.Enums;

/// <summary>
/// Perfil de consumo de recursos. High es el primero a propósito: es el
/// valor por defecto (0) al deserializar preferencias guardadas antes de que
/// existiera esta opción.
/// </summary>
public enum PerformanceMode
{
    High,
    Low
}
