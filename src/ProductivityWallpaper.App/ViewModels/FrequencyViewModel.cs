using ProductivityWallpaper.App.Common;
using ProductivityWallpaper.App.Localization;
using ProductivityWallpaper.App.Services;
using ProductivityWallpaper.Core.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProductivityWallpaper.App.ViewModels;

public class FrequencyOption : ViewModelBase
{
    private bool _isSelected;

    public required WallpaperFrequency Frequency { get; init; }
    public required string DisplayName { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public class FrequencyViewModel : ViewModelBase
{
    public string Title { get; }
    public string ContinueText { get; }
    public string BackText { get; }
    public int StepNumber { get; }
    public int TotalSteps { get; }
    public ObservableCollection<FrequencyOption> Options { get; }
    public ICommand ContinueCommand { get; }
    public ICommand BackCommand { get; }

    public FrequencyViewModel(
        ILocalizationService localization,
        WallpaperFrequency? preselected,
        Action<WallpaperFrequency> onContinue,
        Action onBack,
        int stepNumber = 4,
        int totalSteps = 5)
    {
        StepNumber = stepNumber;
        TotalSteps = totalSteps;
        Title = localization.Get("frequency.title");
        ContinueText = localization.Get("frequency.continue");
        BackText = localization.Get("frequency.back");

        Options = new ObservableCollection<FrequencyOption>(
            Enum.GetValues<WallpaperFrequency>().Select(frequency => new FrequencyOption
            {
                Frequency = frequency,
                DisplayName = localization.Get(frequency.LocalizationKey()),
                IsSelected = frequency == preselected
            }));

        ContinueCommand = new RelayCommand(
            _ => onContinue(Options.First(o => o.IsSelected).Frequency),
            _ => Options.Any(o => o.IsSelected));

        BackCommand = new RelayCommand(_ => onBack());
    }
}
