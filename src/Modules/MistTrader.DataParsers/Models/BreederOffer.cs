using System.Text.Json.Serialization;

namespace DataParsers.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
public record BreederOffer
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }
    
    [JsonPropertyName("breederId")]
    public required long BreederId { get; init; }
    
    [JsonPropertyName("itemId")]
    public required string ItemId { get; init; }
    
    [JsonPropertyName("silver")]
    public required long Silver { get; init; }
}