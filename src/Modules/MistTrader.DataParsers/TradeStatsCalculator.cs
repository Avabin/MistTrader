
namespace DataParsers;

public static class TradeStatsCalculator
{
    public static Dictionary<string, TransactionStats> CalculateStats(IEnumerable<Transaction> transactions, long myBreederId)
    {
        var stats = new Dictionary<string, TransactionStats>();

        foreach (var transaction in transactions)
        {
            var itemId = transaction.SellOffer.ItemId;
            var isBuying = transaction.BuyOffer.BreederId == myBreederId;
            var isSelling = transaction.SellOffer.BreederId == myBreederId;

            if (!stats.TryGetValue(itemId, out var currentStats))
            {
                stats[itemId] = new TransactionStats
                {
                    ItemId = itemId,
                    TotalCount = transaction.Count,
                    TransactionCount = 1,
                    MinPrice = transaction.Silver,
                    MaxPrice = transaction.Silver,
                    TotalVolume = transaction.Silver * transaction.Count,
                    AveragePrice = transaction.Silver,
                    FirstTransaction = transaction.CreatedAt,
                    LastTransaction = transaction.CreatedAt,
                    TotalBought = isBuying ? transaction.Count : 0,
                    TotalSpent = isBuying ? transaction.Count * transaction.Silver : 0,
                    TotalSold = isSelling ? transaction.Count : 0,
                    TotalEarned = isSelling ? transaction.Count * transaction.Silver : 0,
                    ProfitLoss = (isSelling ? transaction.Count * transaction.Silver : 0) - 
                                (isBuying ? transaction.Count * transaction.Silver : 0)
                };
                continue;
            }

            var newTotalCount = currentStats.TotalCount + transaction.Count;
            var newTotalVolume = currentStats.TotalVolume + (transaction.Silver * transaction.Count);

            stats[itemId] = currentStats with
            {
                TotalCount = newTotalCount,
                TransactionCount = currentStats.TransactionCount + 1,
                MinPrice = Math.Min(currentStats.MinPrice, transaction.Silver),
                MaxPrice = Math.Max(currentStats.MaxPrice, transaction.Silver),
                TotalVolume = newTotalVolume,
                AveragePrice = (double)newTotalVolume / newTotalCount,
                FirstTransaction = transaction.CreatedAt < currentStats.FirstTransaction 
                    ? transaction.CreatedAt 
                    : currentStats.FirstTransaction,
                LastTransaction = transaction.CreatedAt > currentStats.LastTransaction 
                    ? transaction.CreatedAt 
                    : currentStats.LastTransaction,
                TotalBought = currentStats.TotalBought + (isBuying ? transaction.Count : 0),
                TotalSpent = currentStats.TotalSpent + (isBuying ? transaction.Count * transaction.Silver : 0),
                TotalSold = currentStats.TotalSold + (isSelling ? transaction.Count : 0),
                TotalEarned = currentStats.TotalEarned + (isSelling ? transaction.Count * transaction.Silver : 0),
                ProfitLoss = (currentStats.TotalEarned + (isSelling ? transaction.Count * transaction.Silver : 0)) -
                            (currentStats.TotalSpent + (isBuying ? transaction.Count * transaction.Silver : 0))
            };
        }

        return stats;
    }
}