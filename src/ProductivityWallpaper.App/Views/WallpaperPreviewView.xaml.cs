using System.Windows.Controls;
using ProductivityWallpaper.App.Presentation;

namespace ProductivityWallpaper.App.Views;

public partial class WallpaperPreviewView : UserControl
{
    public WallpaperPreviewView()
    {
        InitializeComponent();
        ViewTransitions.AttachEntrance(Root);
    }
}
