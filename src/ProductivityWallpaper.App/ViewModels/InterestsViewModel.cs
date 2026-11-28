using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Localization;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

public class InterestOption : ViewModelBase
{
    private bool _isSelected;

    public required InterestCategory Category { get; init; }
    public required string DisplayName { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public class InterestsViewModel : ViewModelBase
{
    public string Title { get; }
    public string ContinueText { get; }
    public string BackText { get; }
    public int StepNumber { get; }
    public int TotalSteps { get; }
    public ObservableCollection<InterestOption> Options { get; }
    public ICommand ContinueCommand { get; }
    public ICommand BackCommand { get; }

    public InterestsViewModel(
        ILocalizationService localization,
        IReadOnlyCollection<InterestCategory> preselected,
        Action<List<InterestCategory>> onContinue,
        Action onBack,
        int stepNumber = 2,
        int totalSteps = 5)
    {
        StepNumber = stepNumber;
        TotalSteps = totalSteps;
        Title = localization.Get("interests.title");
        ContinueText = localization.Get("interests.continue");
        BackText = localization.Get("interests.back");

        Options = new ObservableCollection<InterestOption>(
            Enum.GetValues<InterestCategory>().Select(category => new InterestOption
            {
                Category = category,
                DisplayName = localization.Get(category.LocalizationKey()),
                IsSelected = preselected.Contains(category)
            }));

        // No permitir continuar si no se seleccionó ningún interés.
        ContinueCommand = new RelayCommand(
            _ => onContinue(Options.Where(o => o.IsSelected).Select(o => o.Category).ToList()),
            _ => Options.Any(o => o.IsSelected));

        BackCommand = new RelayCommand(_ => onBack());
    }
}
