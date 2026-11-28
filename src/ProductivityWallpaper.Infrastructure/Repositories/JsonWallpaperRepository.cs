using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProductivityWallpaper.Core.Interfaces;
using ProductivityWallpaper.Core.Models;

namespace ProductivityWallpaper.Infrastructure.Repositories;

/// <summary>
/// Lee el catálogo desde el JSON embebido en este assembly (Content/wallpapers.json)
/// y lo mantiene en memoria. Reemplazar esto en el futuro por otra fuente
/// (archivos externos, wallpapers profesionales, etc.) no afecta a Core ni a App.
/// </summary>
public class JsonWallpaperRepository : IWallpaperRepository
{
    private const string ResourceName = "ProductivityWallpaper.Infrastructure.Content.wallpapers.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly List<WallpaperContent> _catalog;

    public JsonWallpaperRepository()
    {
        _catalog = LoadFromEmbeddedResource();
    }

    public IReadOnlyList<WallpaperContent> GetAll() => _catalog;

    public WallpaperContent? GetById(string id) => _catalog.FirstOrDefault(w => w.Id == id);

    private static List<WallpaperContent> LoadFromEmbeddedResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"No se encontró el recurso embebido '{ResourceName}'.");

        var items = JsonSerializer.Deserialize<List<WallpaperContent>>(stream, SerializerOptions);
        return items ?? new List<WallpaperContent>();
    }
}
