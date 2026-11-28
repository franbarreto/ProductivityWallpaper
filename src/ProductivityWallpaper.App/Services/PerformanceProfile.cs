using System.Windows.Media.Effects;
using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.App.Services;

/// <summary>
/// Estado global del modo de rendimiento. Lo consultan el caché de imágenes,
/// las animaciones de entrada y los widgets; y al cambiar de modo reemplaza
/// en caliente las sombras de Styles.xaml (que se leen con DynamicResource,
/// así que la UI se actualiza sola sin reconstruirse).
///
/// Bajo: sin sombras (DropShadowEffect se renderiza por software en WPF y es
/// lo más caro de la UI), sin animaciones de entrada, imágenes decodificadas
/// más chicas y caché corta.
/// Alto: todo lo anterior activado, imágenes más nítidas y caché más amplia.
/// </summary>
public static class PerformanceProfile
{
    private static readonly string[] ShadowKeys = { "CardShadow", "SoftShadow", "TinyShadow" };

    public static PerformanceMode Mode { get; private set; } = PerformanceMode.High;

    public static bool IsLow => Mode == PerformanceMode.Low;

    public static bool AnimationsEnabled => !IsLow;

    public static int ImageDecodeWidth => IsLow ? 640 : 1280;

    public static int ImageCacheCapacity => IsLow ? 4 : 16;

    public static void Apply(PerformanceMode mode)
    {
        var changed = mode != Mode;
        Mode = mode;

        var resources = Application.Current?.Resources;
        if (resources is not null)
        {
            foreach (var key in ShadowKeys)
            {
                if (IsLow)
                {
                    SetNoEffect(resources, key);
                }
                else
                {
                    // Quitar el override deja de nuevo visible el valor
                    // original definido en Styles.xaml.
                    resources.Remove(key);
                }
            }
        }

        if (changed)
        {
            ImageCache.Clear();
        }
    }

    private static void SetNoEffect(System.Windows.ResourceDictionary resources, string key)
    {
        try
        {
            // null equivale a <x:Null/>: "sin efecto".
            resources[key] = null;
        }
        catch (ArgumentException)
        {
            // Respaldo por si algún día null no se acepta como valor: una
            // sombra totalmente invisible.
            var inert = new DropShadowEffect { Opacity = 0, BlurRadius = 0, ShadowDepth = 0 };
            inert.Freeze();
            resources[key] = inert;
        }
    }
}
