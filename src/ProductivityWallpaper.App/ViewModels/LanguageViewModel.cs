using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

public class LanguageOption
{
    public required AppLanguage Language { get; init; }
    public required string Flag { get; init; }
    public required string DisplayName { get; init; }
}

public class LanguageViewModel : ViewModelBase
{
    public string Title { get; }
    public string BackText { get; }
    public int StepNumber => 1;
    public int TotalSteps => 5;
    public ObservableCollection<LanguageOption> Options { get; }
    public ICommand SelectCommand { get; }
    public ICommand BackCommand { get; }

    public LanguageViewModel(ILocalizationService localization, Action<AppLanguage> onSelected, Action onBack)
    {
        Title = localization.Get("language.title");
        BackText = localization.Get("interests.back");

        Options = new ObservableCollection<LanguageOption>
        {
            new() { Language = AppLanguage.Es, Flag = "🇪🇸", DisplayName = localization.Get("language.es") },
            new() { Language = AppLanguage.En, Flag = "🇺🇸", DisplayName = localization.Get("language.en") },
        };

        SelectCommand = new RelayCommand(parameter => onSelected((AppLanguage)parameter!));
        BackCommand = new RelayCommand(_ => onBack());
    }
}
