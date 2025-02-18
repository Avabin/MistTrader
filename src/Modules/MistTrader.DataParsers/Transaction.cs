using System.Text.Json.Serialization;

namespace DataParsers;

public readonly record struct Transaction
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    
    [JsonPropertyName("maker")]
    public required string Maker { get; init; }
    
    [JsonPropertyName("sellOffer")]
    public required BreederOffer SellOffer { get; init; }
    
    [JsonPropertyName("buyOffer")]
    public required BreederOffer BuyOffer { get; init; }
    
    [JsonPropertyName("count")]
    public required int Count { get; init; }
    
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }
    
    [JsonPropertyName("silver")]
    public required int Silver { get; init; }
}