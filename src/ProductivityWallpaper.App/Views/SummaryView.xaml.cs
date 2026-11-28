using System.Windows.Controls;
using ProductivityWallpaper.App.Presentation;

namespace ProductivityWallpaper.App.Views;

public partial class SummaryView : UserControl
{
    public SummaryView()
    {
        InitializeComponent();
        ViewTransitions.AttachEntrance(Root);
    }
}
