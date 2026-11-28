using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using ProductivityWallpaper.App.Services;

namespace ProductivityWallpaper.App.Widgets;

public partial class DesktopWidgetWindow : Window
{
    private const int GwlExStyle = -20;
    private const int WsExToolWindow = 0x00000080;   // no aparece en Alt+Tab
    private const int WsExNoActivate = 0x08000000;   // hacer clic no le roba el foco a otras apps

    private readonly double? _savedLeft;
    private readonly double? _savedTop;
    private readonly int _slot;

    /// <summary>Se dispara al terminar de arrastrar el widget, con su nueva posición.</summary>
    public event Action<double, double>? Moved;

    public DesktopWidgetWindow(WidgetViewModelBase viewModel, double? savedLeft, double? savedTop, int slot)
    {
        InitializeComponent();

        _savedLeft = savedLeft;
        _savedTop = savedTop;
        _slot = slot;

        Content = viewModel;

        SourceInitialized += OnSourceInitialized;
        Loaded += (_, _) =>
        {
            ApplyPosition();
            PlayOpenAnimation();
        };
        MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        var style = GetWindowLong(handle, GwlExStyle);
        SetWindowLong(handle, GwlExStyle, style | WsExToolWindow | WsExNoActivate);
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            DragMove();
        }
        catch (InvalidOperationException)
        {
            // DragMove solo es válido mientras el botón sigue apretado.
            return;
        }

        Moved?.Invoke(Left, Top);
    }

    /// <summary>
    /// Aparición suave: el widget se desvanece y sube unos píxeles hasta su
    /// lugar. FillBehavior.Stop deja la propiedad en su valor real al terminar,
    /// así no queda "tomada" por la animación y arrastrar el widget sigue
    /// funcionando. En modo de bajo rendimiento aparece directamente.
    /// </summary>
    private void PlayOpenAnimation()
    {
        if (!PerformanceProfile.AnimationsEnabled)
        {
            return;
        }

        var target = Top;
        BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(380))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        });
        BeginAnimation(TopProperty, new DoubleAnimation(target + 16, target, TimeSpan.FromMilliseconds(420))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        });
    }

    /// <summary>
    /// Posición guardada (si hay) o, por defecto, arriba a la derecha del
    /// área de trabajo, un widget debajo del otro. Siempre se corrige para
    /// que quede dentro de la pantalla (por si se desconectó un monitor).
    /// </summary>
    private void ApplyPosition()
    {
        var workArea = SystemParameters.WorkArea;
        var left = _savedLeft ?? workArea.Right - ActualWidth - 48;
        var top = _savedTop ?? workArea.Top + 64 + _slot * 190;

        var minLeft = SystemParameters.VirtualScreenLeft;
        var minTop = SystemParameters.VirtualScreenTop;
        var maxLeft = minLeft + SystemParameters.VirtualScreenWidth - ActualWidth;
        var maxTop = minTop + SystemParameters.VirtualScreenHeight - ActualHeight;

        Left = Math.Max(minLeft, Math.Min(left, maxLeft));
        Top = Math.Max(minTop, Math.Min(top, maxTop));
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
