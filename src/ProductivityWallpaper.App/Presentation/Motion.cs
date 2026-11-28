using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using ProductivityWallpaper.App.Services;

namespace ProductivityWallpaper.App.Presentation;

/// <summary>
/// Comportamientos de animación reutilizables desde XAML (propiedades
/// adjuntas). Todos consultan PerformanceProfile en el momento de animar, así
/// que cambiar a modo de bajo rendimiento los apaga al instante.
/// </summary>
public static class Motion
{
    /// <summary>
    /// En una barra de progreso: en vez de saltar al nuevo valor, lo recorre
    /// con una animación suave. Se usa en vez de enlazar Value directamente.
    /// </summary>
    public static readonly DependencyProperty AnimatedValueProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedValue",
            typeof(double),
            typeof(Motion),
            new PropertyMetadata(0.0, OnAnimatedValueChanged));

    public static double GetAnimatedValue(DependencyObject element) =>
        (double)element.GetValue(AnimatedValueProperty);

    public static void SetAnimatedValue(DependencyObject element, double value) =>
        element.SetValue(AnimatedValueProperty, value);

    private static void OnAnimatedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RangeBase bar)
        {
            return;
        }

        var target = (double)e.NewValue;

        if (!PerformanceProfile.AnimationsEnabled || !bar.IsLoaded)
        {
            // Sin animación (modo bajo, o primer valor antes de mostrarse): se
            // suelta cualquier animación en curso y se asigna directo.
            bar.BeginAnimation(RangeBase.ValueProperty, null);
            bar.Value = target;
            return;
        }

        bar.BeginAnimation(RangeBase.ValueProperty, new DoubleAnimation(target, TimeSpan.FromMilliseconds(700))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        });
    }
}
