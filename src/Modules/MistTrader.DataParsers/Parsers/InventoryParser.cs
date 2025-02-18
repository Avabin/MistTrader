using System.Text.Json;
using DataParsers.Models;

namespace DataParsers.Parsers;

public class InventoryParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Synchronously parses inventory items from JSON string
    /// </summary>
    public static IReadOnlyList<InventoryItem> ParseInventory(string json)
    {
        try
        {
            var items = JsonSerializer.Deserialize<List<InventoryItem>>(json, Options);
            return items ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    /// <summary>
    /// Synchronously parses inventory items from JSON file
    /// </summary>
    public static IReadOnlyList<InventoryItem> ParseFile(string filePath)
    {
        if (!File.Exists(filePath))
            return [];

        try
        {
            var json = File.ReadAllText(filePath);
            return ParseInventory(json);
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// Asynchronously parses inventory items from a stream
    /// </summary>
    public static async ValueTask<IReadOnlyList<InventoryItem>> ParseInventoryAsync(Stream stream)
    {
        try
        {
            var items = await JsonSerializer.DeserializeAsync<List<InventoryItem>>(stream, Options);
            return items ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    /// <summary>
    /// Asynchronously parses inventory items from JSON file
    /// </summary>
    public static async ValueTask<IReadOnlyList<InventoryItem>> ParseFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return [];

        try
        {
            await using var stream = File.OpenRead(filePath);
            return await ParseInventoryAsync(stream);
        }
        catch (Exception)
        {
            return [];
        }
    }
}