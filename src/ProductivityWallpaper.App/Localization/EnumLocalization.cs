using ProductivityWallpaper.Core.Enums;

namespace ProductivityWallpaper.App.Localization;

public static class EnumLocalization
{
    public static string LocalizationKey(this InterestCategory category) => category switch
    {
        InterestCategory.Discipline => "category.discipline",
        InterestCategory.Motivation => "category.motivation",
        InterestCategory.Focus => "category.focus",
        InterestCategory.Study => "category.study",
        InterestCategory.Work => "category.work",
        InterestCategory.Training => "category.training",
        InterestCategory.Productivity => "category.productivity",
        InterestCategory.Stoicism => "category.stoicism",
        InterestCategory.Faith => "category.faith",
        InterestCategory.SelfImprovement => "category.selfimprovement",
        _ => category.ToString()
    };

    public static string LocalizationKey(this VisualStyle style) => style switch
    {
        VisualStyle.Minimalist => "style.minimalist",
        VisualStyle.Dark => "style.dark",
        VisualStyle.Light => "style.light",
        VisualStyle.Nature => "style.nature",
        VisualStyle.Abstract => "style.abstract",
        VisualStyle.Elegant => "style.elegant",
        _ => style.ToString()
    };

    public static string LocalizationKey(this WallpaperFrequency frequency) => frequency switch
    {
        WallpaperFrequency.Minutes15 => "frequency.minutes15",
        WallpaperFrequency.Minutes30 => "frequency.minutes30",
        WallpaperFrequency.Hour1 => "frequency.hour1",
        WallpaperFrequency.Hours3 => "frequency.hours3",
        WallpaperFrequency.Day1 => "frequency.day1",
        WallpaperFrequency.Manual => "frequency.manual",
        _ => frequency.ToString()
    };
}
