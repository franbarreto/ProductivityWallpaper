using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductivityWallpaper.App.Services;

namespace ProductivityWallpaper.App.Controls;

/// <summary>
/// Fila de puntos "● ● ○ ○ ○" que ubica al usuario dentro del onboarding
/// (Idioma → Intereses → Estilo → Frecuencia → Resumen). Puramente visual:
/// no depende de ningún ViewModel propio, cada pantalla solo le pasa en
/// qué paso está y cuántos hay en total.
/// </summary>
public partial class StepIndicator : UserControl
{
    public static readonly DependencyProperty TotalStepsProperty = DependencyProperty.Register(
        nameof(TotalSteps), typeof(int), typeof(StepIndicator),
        new PropertyMetadata(1, OnStepsChanged));

    public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
        nameof(CurrentStep), typeof(int), typeof(StepIndicator),
        new PropertyMetadata(1, OnStepsChanged));

    public int TotalSteps
    {
        get => (int)GetValue(TotalStepsProperty);
        set => SetValue(TotalStepsProperty, value);
    }

    public int CurrentStep
    {
        get => (int)GetValue(CurrentStepProperty);
        set => SetValue(CurrentStepProperty, value);
    }

    public StepIndicator()
    {
        InitializeComponent();
        Loaded += (_, _) => Rebuild();
    }

    private static void OnStepsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((StepIndicator)d).Rebuild();
    }

    private void Rebuild()
    {
        if (Dots is null)
        {
            return;
        }

        Dots.Children.Clear();

        var accent = (Brush)FindResource("AccentBrush");
        var dim = (Brush)FindResource("BorderBrush");

        for (var step = 1; step <= TotalSteps; step++)
        {
            var isCurrent = step == CurrentStep;
            var isDone = step < CurrentStep;

            var dot = new Ellipse
            {
                Width = isCurrent ? 9 : 7,
                Height = isCurrent ? 9 : 7,
                Margin = new Thickness(4, 0, 4, 0),
                Fill = isCurrent || isDone ? accent : dim,
                Opacity = isCurrent ? 1.0 : (isDone ? 0.8 : 0.5),
                VerticalAlignment = VerticalAlignment.Center,
            };

            if (isCurrent && PerformanceProfile.AnimationsEnabled)
            {
                var pulse = new DoubleAnimation(0.55, 1.0, TimeSpan.FromMilliseconds(600))
                {
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(1),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                dot.BeginAnimation(UIElement.OpacityProperty, pulse);
            }

            Dots.Children.Add(dot);
        }
    }
}
