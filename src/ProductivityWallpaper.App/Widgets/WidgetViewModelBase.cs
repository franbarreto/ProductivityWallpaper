using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.App.Widgets;

/// <summary>
/// Parte común de todos los widgets de escritorio: apariencia (tamaño,
/// tarjeta translúcida o texto directo sobre el wallpaper, sombra del texto)
/// y el contrato de actualización por reloj.
/// </summary>
public abstract class WidgetViewModelBase : ViewModelBase
{
    // Tarjeta oscura translúcida: legible sobre cualquier wallpaper y, a
    // diferencia de una sombra, no cuesta nada de renderizado.
    private static readonly Brush CardBrush = CreateCardBrush();
    private static readonly Effect TextShadow = CreateTextShadow();

    private double _scale = 1.0;
    public double Scale
    {
        get => _scale;
        private set => SetProperty(ref _scale, value);
    }

    private Brush _cardBackground = Brushes.Transparent;
    public Brush CardBackground
    {
        get => _cardBackground;
        private set => SetProperty(ref _cardBackground, value);
    }

    private Effect? _textEffect;
    public Effect? TextEffect
    {
        get => _textEffect;
        private set => SetProperty(ref _textEffect, value);
    }

    private Thickness _cardPadding = new(24, 18, 24, 18);
    public Thickness CardPadding
    {
        get => _cardPadding;
        private set => SetProperty(ref _cardPadding, value);
    }

    private CornerRadius _cardCornerRadius = new(26);
    public CornerRadius CardCornerRadius
    {
        get => _cardCornerRadius;
        private set => SetProperty(ref _cardCornerRadius, value);
    }

    /// <summary>Recalcula el contenido para la hora dada.</summary>
    public abstract void Tick(DateTime now);

    public void ApplyAppearance(WidgetSize size, bool hasCard, bool useTextShadow)
    {
        Scale = size switch
        {
            WidgetSize.Small => 0.75,
            WidgetSize.Large => 1.6,
            _ => 1.0
        };

        CardBackground = hasCard ? CardBrush : Brushes.Transparent;
        TextEffect = !hasCard && useTextShadow ? TextShadow : null;
        CardPadding = hasCard
            ? new Thickness(24 * Scale, 18 * Scale, 24 * Scale, 18 * Scale)
            : new Thickness(8 * Scale);
        CardCornerRadius = new CornerRadius(26 * Scale);

        OnAppearanceChanged();
    }

    /// <summary>Los widgets concretos recalculan tamaños de fuente según Scale.</summary>
    protected abstract void OnAppearanceChanged();

    private static Brush CreateCardBrush()
    {
        var brush = new SolidColorBrush(Color.FromArgb(0xA6, 0x14, 0x14, 0x18));
        brush.Freeze();
        return brush;
    }

    private static Effect CreateTextShadow()
    {
        var shadow = new DropShadowEffect
        {
            Color = Color.FromRgb(0, 0, 0),
            Opacity = 0.55,
            BlurRadius = 10,
            ShadowDepth = 1,
            Direction = 270
        };
        shadow.Freeze();
        return shadow;
    }
}
