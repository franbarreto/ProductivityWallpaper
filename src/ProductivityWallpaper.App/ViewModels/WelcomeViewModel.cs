using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Services;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

public class WelcomeViewModel : ViewModelBase
{
    public string Title { get; }
    public string Subtitle { get; }
    public string StartText { get; }
    public ICommand StartCommand { get; }

    public WelcomeViewModel(ILocalizationService localization, Action onStart)
    {
        Title = localization.Get("welcome.title");
        Subtitle = localization.Get("welcome.subtitle");
        StartText = localization.Get("welcome.start");
        StartCommand = new RelayCommand(_ => onStart());
    }
}
