using System.Windows.Controls;
using ProductivityWallpaper.App.Presentation;

namespace ProductivityWallpaper.App.Views;

public partial class LanguageView : UserControl
{
    public LanguageView()
    {
        InitializeComponent();
        ViewTransitions.AttachEntrance(Root);
    }
}
