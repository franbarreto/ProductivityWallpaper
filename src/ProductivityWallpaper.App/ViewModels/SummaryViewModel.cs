using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Localization;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;
using ProductivityWallpaper.Core.Models;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

public class SummaryViewModel : ViewModelBase
{
    public string Title { get; }
    public int StepNumber => 5;
    public int TotalSteps => 5;
    public string LanguageLabel { get; }
    public string LanguageValue { get; }
    public string InterestsLabel { get; }
    public string InterestsValue { get; }
    public string StyleLabel { get; }
    public string StyleValue { get; }
    public string FrequencyLabel { get; }
    public string FrequencyValue { get; }
    public string ConfirmText { get; }
    public string BackText { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand BackCommand { get; }

    public SummaryViewModel(ILocalizationService localization, UserPreferences draft, Action onConfirm, Action onBack)
    {
        Title = localization.Get("summary.title");

        LanguageLabel = localization.Get("summary.language");
        LanguageValue = draft.Language == AppLanguage.Es
            ? localization.Get("language.es")
            : localization.Get("language.en");

        InterestsLabel = localization.Get("summary.interests");
        InterestsValue = string.Join(", ", draft.SelectedCategories.Select(c => localization.Get(c.LocalizationKey())));

        StyleLabel = localization.Get("summary.style");
        StyleValue = draft.SelectedStyle.HasValue ? localization.Get(draft.SelectedStyle.Value.LocalizationKey()) : "-";

        FrequencyLabel = localization.Get("summary.frequency");
        FrequencyValue = draft.WallpaperInterval.HasValue
            ? localization.Get(draft.WallpaperInterval.Value.LocalizationKey())
            : "-";

        ConfirmText = localization.Get("summary.confirm");
        BackText = localization.Get("summary.back");

        ConfirmCommand = new RelayCommand(_ => onConfirm());
        BackCommand = new RelayCommand(_ => onBack());
    }
}
