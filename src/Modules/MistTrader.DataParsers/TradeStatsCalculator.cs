using DataParsers.Models;

namespace DataParsers;

public static class TradeStatsCalculator
{
    public static Dictionary<string, TransactionStats> CalculateStats(IEnumerable<Transaction> transactions)
    {
        var stats = new Dictionary<string, TransactionStats>();

        foreach (var transaction in transactions)
        {
            var itemId = transaction.SellOffer.ItemId;

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
                    TotalBought = 0,
                    TotalSpent = 0,
                    TotalSold = 0,
                    TotalEarned = 0,
                    ProfitLoss = 0
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
                    : currentStats.LastTransaction
            };
        }

        return stats;
    }

    public static Dictionary<string, TransactionStats> CalculatePersonalStats(
        this Dictionary<string, TransactionStats> marketStats,
        IEnumerable<Transaction> transactions,
        long breederId)
    {
        var personalStats = new Dictionary<string, TransactionStats>();

        foreach (var (itemId, marketStat) in marketStats)
        {
            var itemTransactions = transactions.Where(t => t.SellOffer.ItemId == itemId).ToList();
            var bought = itemTransactions
                .Where(t => t.BuyOffer.BreederId == breederId)
                .ToList();
            var sold = itemTransactions
                .Where(t => t.SellOffer.BreederId == breederId)
                .ToList();

            var totalBought = bought.Sum(t => t.Count);
            var totalSpent = bought.Sum(t => t.Count * t.Silver);
            var totalSold = sold.Sum(t => t.Count);
            var totalEarned = sold.Sum(t => t.Count * t.Silver);

            personalStats[itemId] = marketStat with
            {
                TotalBought = totalBought,
                TotalSpent = totalSpent,
                TotalSold = totalSold,
                TotalEarned = totalEarned,
                ProfitLoss = totalEarned - totalSpent
            };
        }

        return personalStats;
    }
}