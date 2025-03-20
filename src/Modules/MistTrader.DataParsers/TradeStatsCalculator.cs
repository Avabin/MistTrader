using System.Collections.Concurrent;
using DataParsers.Models;
using System.Linq;

namespace DataParsers;

public static class TradeStatsCalculator
{
    public static Dictionary<string, TransactionStats> CalculateStatsA(IEnumerable<Transaction> transactions)
    {
        var stats = new Dictionary<string, TransactionStats>();

        var transactionsArray = transactions as Transaction[] ?? transactions.ToArray();
        foreach (var transaction in transactionsArray)
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
                    MedianPrice = transaction.Silver,
                    StandardDeviation = 0,
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
            var newTransactionCount = currentStats.TransactionCount + 1;
            var newAveragePrice = (double)newTotalVolume / newTotalCount;

            var prices = transactionsArray.Where(t => t.SellOffer.ItemId == itemId).Select(t => t.Silver).OrderBy(p => p).ToList();
            var medianPrice = prices.Count % 2 == 0
                ? (prices[prices.Count / 2 - 1] + prices[prices.Count / 2]) / 2.0
                : prices[prices.Count / 2];

            var variance = prices.Average(p => Math.Pow(p - newAveragePrice, 2));
            var standardDeviation = Math.Sqrt(variance);

            stats[itemId] = currentStats with
            {
                TotalCount = newTotalCount,
                TransactionCount = newTransactionCount,
                MinPrice = Math.Min(currentStats.MinPrice, transaction.Silver),
                MaxPrice = Math.Max(currentStats.MaxPrice, transaction.Silver),
                TotalVolume = newTotalVolume,
                AveragePrice = newAveragePrice,
                FirstTransaction = transaction.CreatedAt < currentStats.FirstTransaction
                    ? transaction.CreatedAt
                    : currentStats.FirstTransaction,
                LastTransaction = transaction.CreatedAt > currentStats.LastTransaction
                    ? transaction.CreatedAt
                    : currentStats.LastTransaction,
                MedianPrice = medianPrice,
                StandardDeviation = standardDeviation
            };
        }

        return stats;
    }

    public static Dictionary<string, TransactionStats> CalculateStats(IReadOnlyList<Transaction> transactions)
    {
        var grouped = transactions.GroupBy(t => t.SellOffer.ItemId).ToArray();
        
        var concurrentDictionary = new ConcurrentDictionary<string, TransactionStats>();
        
        Parallel.ForEach(grouped, g =>
        {
            var stats = TransactionStats.Empty.Apply(g.ToList());
            
            concurrentDictionary.TryAdd(g.Key, stats);
        });
        
        return concurrentDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    
    public static TransactionStats CalculateStat(IReadOnlyList<Transaction> transactions)
    {
        // only for single itemid
        var itemIds = transactions.Select(t => t.SellOffer.ItemId).Distinct().ToArray();
        if (itemIds.Length > 1)
        {
            throw new InvalidOperationException("Only one itemid is allowed");
        }
        
        var stats = TransactionStats.Empty.Apply(transactions);
        
        return stats;
    }

    public static Dictionary<string, TransactionStats> CalculatePersonalStats(
        this Dictionary<string, TransactionStats> marketStats,
        IEnumerable<Transaction> transactions,
        long breederId)
    {
        var personalStats = new Dictionary<string, TransactionStats>();

        var transactionsArray = transactions as Transaction[] ?? transactions.ToArray();
        foreach (var (itemId, marketStat) in marketStats)
        {
            var itemTransactions = transactionsArray.Where(t => t.SellOffer.ItemId == itemId).ToList();
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

    public static InventoryStatistics CalculateInventoryStats(IEnumerable<InventoryItem> inventoryItems)
    {
        var inventoryStatistics = new InventoryStatistics();
        inventoryStatistics.Calculate(inventoryItems);
        return inventoryStatistics;
    }
}