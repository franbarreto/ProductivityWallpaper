using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ProductivityWallpaper.App.Services;

namespace ProductivityWallpaper.App.Presentation;

/// <summary>
/// Animación de entrada compartida (fade + leve slide-up) para la raíz de
/// cada pantalla. Centralizada acá para no repetir Storyboards en cada
/// View.xaml, y para poder ajustar el "feel" general en un solo lugar.
/// </summary>
public static class ViewTransitions
{
    public static void AttachEntrance(FrameworkElement root)
    {
        // Modo de bajo rendimiento: sin animación de entrada, la pantalla
        // aparece directamente.
        if (!PerformanceProfile.AnimationsEnabled)
        {
            return;
        }

        root.Opacity = 0;
        var transform = new TranslateTransform(0, 14);
        root.RenderTransform = transform;

        root.Loaded += (_, _) =>
        {
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(260))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var slide = new DoubleAnimation(14, 0, TimeSpan.FromMilliseconds(320))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            root.BeginAnimation(UIElement.OpacityProperty, fade);
            transform.BeginAnimation(TranslateTransform.YProperty, slide);
        };
    }
}
