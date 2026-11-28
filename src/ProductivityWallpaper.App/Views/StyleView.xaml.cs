using System.Windows.Controls;
using ProductivityWallpaper.App.Presentation;

namespace ProductivityWallpaper.App.Views;

public partial class StyleView : UserControl
{
    public StyleView()
    {
        InitializeComponent();
        ViewTransitions.AttachEntrance(Root);
    }
}
