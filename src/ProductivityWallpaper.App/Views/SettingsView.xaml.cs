using System.Windows.Controls;
using ProductivityWallpaper.App.Presentation;

namespace ProductivityWallpaper.App.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        ViewTransitions.AttachEntrance(Root);
    }
}
