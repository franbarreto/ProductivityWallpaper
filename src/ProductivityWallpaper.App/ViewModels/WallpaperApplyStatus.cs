namespace ProductivityWallpaper.App.ViewModels;

public enum WallpaperApplyStatus
{
    Applied,
    ImageMissing,
    Failed,
    NoContent,

    /// <summary>
    /// Solo se actualizó la vista previa (panel de prueba de preferencias);
    /// no se tocó el fondo real de Windows.
    /// </summary>
    PreviewOnly
}
