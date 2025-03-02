using System.Text.Json.Serialization;

namespace DataParsers.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
public record Transaction
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }
    
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
    public required long Silver { get; init; }
    
    [JsonIgnore]
    public static Transaction Empty => new Transaction
    {
        Id = 0,
        Maker = string.Empty,
        SellOffer = BreederOffer.Empty,
        BuyOffer = BreederOffer.Empty,
        Count = 0,
        CreatedAt = DateTime.MinValue,
        Silver = 0
    };

    public static bool IsEmpty(Transaction transaction) => transaction.Equals(Empty);
}