using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using DataParsers.Models;

namespace DataParsers.Parsers;

public class ReactiveTradesParser : IReactiveTradesParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = JsonContext.Default
    };

    private const long MyBreederId = 6305; // We could get this from configuration later

    public IObservable<Dictionary<string, TransactionStats>> CalculateStatsReactive(Stream stream)
    {
        return ParseTransactionsReactive(stream)
            .Scan(
                new Dictionary<string, TransactionStats>(),
                (stats, transaction) =>
                {
                    var newStats = new Dictionary<string, TransactionStats>(stats);
                    var itemId = transaction.SellOffer.ItemId;

                    // Check if I'm the buyer (my breederId in buyOffer) or seller (my breederId in sellOffer)
                    var isBuying = transaction.BuyOffer.BreederId == MyBreederId;
                    var isSelling = transaction.SellOffer.BreederId == MyBreederId;

                    if (!newStats.TryGetValue(itemId, out var currentStats))
                    {
                        var totalBought = isBuying ? transaction.Count : 0;
                        var totalSpent = isBuying ? transaction.Count * transaction.Silver : 0;
                        var totalSold = isSelling ? transaction.Count : 0;
                        var totalEarned = isSelling ? transaction.Count * transaction.Silver : 0;

                        currentStats = new TransactionStats
                        {
                            ItemId = itemId,
                            FirstTransaction = transaction.CreatedAt,
                            LastTransaction = transaction.CreatedAt,
                            MinPrice = transaction.Silver,
                            MaxPrice = transaction.Silver,
                            TotalCount = transaction.Count,
                            TotalVolume = transaction.Silver * transaction.Count,
                            TransactionCount = 1,
                            AveragePrice = transaction.Silver,
                            TotalBought = totalBought,
                            TotalSpent = totalSpent,
                            TotalSold = totalSold,
                            TotalEarned = totalEarned,
                            ProfitLoss = totalEarned - totalSpent,
                            TotalTransactionValue = transaction.Silver * transaction.Count
                        };
                    }
                    else
                    {
                        var totalBought = currentStats.TotalBought + (isBuying ? transaction.Count : 0);
                        var totalSpent = currentStats.TotalSpent +
                                         (isBuying ? transaction.Count * transaction.Silver : 0);
                        var totalSold = currentStats.TotalSold + (isSelling ? transaction.Count : 0);
                        var totalEarned = currentStats.TotalEarned +
                                          (isSelling ? transaction.Count * transaction.Silver : 0);
                        var newTotalCount = currentStats.TotalCount + transaction.Count;
                        var newTotalVolume = currentStats.TotalVolume + (transaction.Silver * transaction.Count);

                        currentStats = currentStats with
                        {
                            TotalCount = newTotalCount,
                            TotalVolume = newTotalVolume,
                            AveragePrice = (double)newTotalVolume / newTotalCount,
                            MinPrice = Math.Min(currentStats.MinPrice, transaction.Silver),
                            MaxPrice = Math.Max(currentStats.MaxPrice, transaction.Silver),
                            TransactionCount = currentStats.TransactionCount + 1,
                            FirstTransaction = transaction.CreatedAt < currentStats.FirstTransaction
                                ? transaction.CreatedAt
                                : currentStats.FirstTransaction,
                            LastTransaction = transaction.CreatedAt > currentStats.LastTransaction
                                ? transaction.CreatedAt
                                : currentStats.LastTransaction,
                            TotalBought = totalBought,
                            TotalSpent = totalSpent,
                            TotalSold = totalSold,
                            TotalEarned = totalEarned,
                            ProfitLoss = totalEarned - totalSpent,
                            TotalTransactionValue = newTotalVolume
                        };
                    }

                    newStats[itemId] = currentStats;
                    return newStats;
                }
            );
    }

    public IObservable<Transaction> ParseTransactionsReactive(Stream stream)
    {
        return Observable.Create<Transaction>(observer =>
        {
            try
            {
                var transactions = JsonSerializer.Deserialize<List<Transaction>>(stream, Options);

                if (transactions == null)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                foreach (var transaction in transactions)
                {
                    observer.OnNext(transaction);
                }

                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }

            return Disposable.Empty;
        });
    }
}
