using System.Windows.Controls;
using ProductivityWallpaper.App.Presentation;

namespace ProductivityWallpaper.App.Views;

public partial class InterestsView : UserControl
{
    public InterestsView()
    {
        InitializeComponent();
        ViewTransitions.AttachEntrance(Root);
    }
}
