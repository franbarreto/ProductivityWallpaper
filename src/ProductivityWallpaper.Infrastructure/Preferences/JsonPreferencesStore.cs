using System.Text.Json;
using System.Text.Json.Serialization;
using ProductivityWallpaper.Core.Enums;
using ProductivityWallpaper.Core.Interfaces;
using ProductivityWallpaper.Core.Models;

namespace ProductivityWallpaper.Infrastructure.Preferences;

/// <summary>
/// Guarda las preferencias como JSON (ver AppDataLocation: %LOCALAPPDATA% o,
/// en modo portable, la carpeta Data junto a la app).
///
/// Endurecido para que un archivo dañado, enorme o editado a mano no pueda
/// romper la app:
///  - se rechaza si pesa más de 64 KB;
///  - los valores enumerados desconocidos y las posiciones absurdas de
///    widgets se descartan (Sanitize);
///  - se guarda de forma atómica (archivo temporal + reemplazo), así un corte
///    de luz a mitad de escritura no deja el JSON a medias;
///  - si no se puede escribir (disco de solo lectura, permisos), Save no lanza:
///    la app sigue funcionando con las preferencias en memoria.
/// </summary>
public class JsonPreferencesStore : IPreferencesStore
{
    private const long MaxFileBytes = 64 * 1024;
    private const double MaxCoordinate = 100_000;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;

    public JsonPreferencesStore()
    {
        _filePath = Path.Combine(AppDataLocation.Folder, "preferences.json");
    }

    public void Save(UserPreferences preferences)
    {
        try
        {
            var json = JsonSerializer.Serialize(preferences, SerializerOptions);
            var tempPath = _filePath + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(_filePath))
            {
                File.Replace(tempPath, _filePath, null);
            }
            else
            {
                File.Move(tempPath, _filePath);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            // Guardar es "mejor esfuerzo": no vale la pena cerrar la app por esto.
        }
    }

    public UserPreferences? Load()
    {
        try
        {
            var info = new FileInfo(_filePath);
            if (!info.Exists || info.Length > MaxFileBytes)
            {
                return null;
            }

            var json = File.ReadAllText(_filePath);
            var loaded = JsonSerializer.Deserialize<UserPreferences>(json, SerializerOptions);
            return loaded is null ? null : Sanitize(loaded);
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            // Archivo corrupto o ilegible: se trata como si no hubiera preferencias guardadas.
            return null;
        }
    }

    /// <summary>Deja solo valores válidos, aunque el archivo haya sido editado a mano.</summary>
    private static UserPreferences Sanitize(UserPreferences p)
    {
        if (!Enum.IsDefined(p.Language)) p.Language = AppLanguage.Es;
        if (!Enum.IsDefined(p.Performance)) p.Performance = PerformanceMode.High;
        if (!Enum.IsDefined(p.WidgetSize)) p.WidgetSize = WidgetSize.Medium;
        if (p.SelectedStyle.HasValue && !Enum.IsDefined(p.SelectedStyle.Value)) p.SelectedStyle = null;
        if (p.WallpaperInterval.HasValue && !Enum.IsDefined(p.WallpaperInterval.Value)) p.WallpaperInterval = null;

        p.SelectedCategories = (p.SelectedCategories ?? new List<InterestCategory>())
            .Where(c => Enum.IsDefined(c))
            .Distinct()
            .ToList();

        p.ClockWidgetLeft = ValidCoordinate(p.ClockWidgetLeft);
        p.ClockWidgetTop = ValidCoordinate(p.ClockWidgetTop);
        p.DayProgressWidgetLeft = ValidCoordinate(p.DayProgressWidgetLeft);
        p.DayProgressWidgetTop = ValidCoordinate(p.DayProgressWidgetTop);
        return p;
    }

    private static double? ValidCoordinate(double? value) =>
        value.HasValue && double.IsFinite(value.Value) && Math.Abs(value.Value) <= MaxCoordinate ? value : null;
}
