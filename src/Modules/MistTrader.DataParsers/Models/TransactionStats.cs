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
    
    public required long TotalBought { get; init; }
    public required long TotalSpent { get; init; }
    public required long TotalSold { get; init; }
    public required long TotalEarned { get; init; }
    public required long ProfitLoss { get; init; }

    public TransactionStats Apply(IReadOnlyList<Transaction> transactions)
    {
        var itemId = transactions.Select(t => t.SellOffer.ItemId).Distinct().Single();
        
        var newTotalCount = this.TotalCount + transactions.Sum(t => t.Count);
        var newTotalVolume = this.TotalVolume + transactions.Sum(t => t.Silver * t.Count);
        var newTransactionCount = this.TransactionCount + transactions.Count;
        var newAveragePrice = (double)newTotalVolume / newTotalCount;
        
        var prices = transactions.Select(t => t.Silver).OrderBy(p => p).ToList();
        var medianPrice = prices.Count % 2 == 0
            ? (prices[prices.Count / 2 - 1] + prices[prices.Count / 2]) / 2.0
            : prices[prices.Count / 2];
        
        var variance = prices.Average(p => Math.Pow(p - newAveragePrice, 2));
        var standardDeviation = Math.Sqrt(variance);
        
        var isFirstTransactionDefault = this.FirstTransaction == DateTime.MinValue;
        var isLastTransactionDefault = this.LastTransaction == DateTime.MinValue;
        var firstTransaction = isFirstTransactionDefault
            ? transactions.Min(t => t.CreatedAt)
            : this.FirstTransaction;
        var lastTransaction = isLastTransactionDefault
            ? transactions.Max(t => t.CreatedAt)
            : this.LastTransaction;
        
        var isMinPriceDefault = this.MinPrice == 0;
        var isMaxPriceDefault = this.MaxPrice == 0;
        
        var minPrice = isMinPriceDefault
            ? transactions.Min(t => t.Silver)
            : this.MinPrice;
        
        var maxPrice = isMaxPriceDefault
            ? transactions.Max(t => t.Silver)
            : this.MaxPrice;
        
        return this with
        {
            ItemId = itemId,
            TotalCount = newTotalCount,
            TransactionCount = newTransactionCount,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            TotalVolume = newTotalVolume,
            AveragePrice = newAveragePrice,
            FirstTransaction = firstTransaction,
            LastTransaction = lastTransaction,
            MedianPrice = medianPrice,
            StandardDeviation = standardDeviation
        };
    }
    
    // empty 
    public static TransactionStats Empty => new TransactionStats
    {
        ItemId = string.Empty,
        TotalCount = 0,
        TransactionCount = 0,
        MinPrice = 0,
        MaxPrice = 0,
        TotalVolume = 0,
        AveragePrice = 0,
        FirstTransaction = DateTime.MinValue,
        LastTransaction = DateTime.MinValue,
        MedianPrice = 0,
        StandardDeviation = 0,
        TotalBought = 0,
        TotalSpent = 0,
        TotalSold = 0,
        TotalEarned = 0,
        ProfitLoss = 0
    };
}
