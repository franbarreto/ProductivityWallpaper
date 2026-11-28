using System.Windows.Controls;
using ProductivityWallpaper.App.Presentation;

namespace ProductivityWallpaper.App.Views;

public partial class FrequencyView : UserControl
{
    public FrequencyView()
    {
        InitializeComponent();
        ViewTransitions.AttachEntrance(Root);
    }
}
