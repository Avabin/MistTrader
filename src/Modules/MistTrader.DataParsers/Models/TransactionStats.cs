using System.Text.Json.Serialization;

namespace DataParsers.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
public record TransactionStats
{
    public required string ItemId { get; init; }
    public required int TotalCount { get; init; }
    public required int TransactionCount { get; init; }
    public required long MinPrice { get; init; }
    public required long MaxPrice { get; init; }
    public required long TotalVolume { get; init; }
    public required double AveragePrice { get; init; }
    public required DateTime FirstTransaction { get; init; }
    public required DateTime LastTransaction { get; init; }
    public required double MedianPrice { get; init; }
    public required double StandardDeviation { get; init; }
    public required long TotalTransactionValue { get; init; }
    
    public required long TotalBought { get; init; }
    public required long TotalSpent { get; init; }
    public required long TotalSold { get; init; }
    public required long TotalEarned { get; init; }
    public required long ProfitLoss { get; init; }
}
