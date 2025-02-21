using System.Text.Json.Serialization;

namespace DataParsers.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
public record InventoryItem
{
    [JsonPropertyName("itemId")]
    public required string ItemId { get; init; }
    
    [JsonPropertyName("count")]
    public required int Count { get; init; }
    
    [JsonPropertyName("level")]
    public required int Level { get; init; }
    
    [JsonPropertyName("isIdentified")]
    public required bool IsIdentified { get; init; }
    
    [JsonPropertyName("soulBound")]
    public required bool SoulBound { get; init; }
    
    [JsonPropertyName("silver")]
    public required long Silver { get; init; }
    
    [JsonPropertyName("totalValue")]
    public long TotalValue => Silver * Count;
    
    [JsonPropertyName("averageValue")]
    public double AverageValue => Count > 0 ? (double)TotalValue / Count : 0;
}
