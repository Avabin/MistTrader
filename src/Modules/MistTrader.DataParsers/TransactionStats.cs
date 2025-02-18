namespace DataParsers;

public readonly record struct TransactionStats
{
    public required string ItemId { get; init; }
    public required int TotalCount { get; init; }
    public required long TotalVolume { get; init; }
    public required double AveragePrice { get; init; }
    public required int MinPrice { get; init; }
    public required int MaxPrice { get; init; }
    public required int TransactionCount { get; init; }
    public required DateTime FirstTransaction { get; init; }
    public required DateTime LastTransaction { get; init; }
}