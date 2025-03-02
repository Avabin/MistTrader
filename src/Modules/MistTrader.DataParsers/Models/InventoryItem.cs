using System.Text.Json.Serialization;

namespace DataParsers.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
public record InventoryItem(
    [property: JsonPropertyName("itemId")] string ItemId,
    [property: JsonPropertyName("count")] int Count);