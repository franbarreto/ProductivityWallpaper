using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Localization;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

public class StyleOption : ViewModelBase
{
    private bool _isSelected;

    public required VisualStyle Style { get; init; }
    public required string DisplayName { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public class StyleViewModel : ViewModelBase
{
    public string Title { get; }
    public string ContinueText { get; }
    public string BackText { get; }
    public int StepNumber { get; }
    public int TotalSteps { get; }
    public ObservableCollection<StyleOption> Options { get; }
    public ICommand ContinueCommand { get; }
    public ICommand BackCommand { get; }

    public StyleViewModel(
        ILocalizationService localization,
        VisualStyle? preselected,
        Action<VisualStyle> onContinue,
        Action onBack,
        int stepNumber = 3,
        int totalSteps = 5)
    {
        StepNumber = stepNumber;
        TotalSteps = totalSteps;
        Title = localization.Get("style.title");
        ContinueText = localization.Get("style.continue");
        BackText = localization.Get("style.back");

        Options = new ObservableCollection<StyleOption>(
            Enum.GetValues<VisualStyle>().Select(style => new StyleOption
            {
                Style = style,
                DisplayName = localization.Get(style.LocalizationKey()),
                IsSelected = style == preselected
            }));

        ContinueCommand = new RelayCommand(
            _ => onContinue(Options.First(o => o.IsSelected).Style),
            _ => Options.Any(o => o.IsSelected));

        BackCommand = new RelayCommand(_ => onBack());
    }
}
