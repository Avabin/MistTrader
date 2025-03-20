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

    public IObservable<Dictionary<string, TransactionStats>> CalculateStatsReactive(Stream stream, long breederId)
    {
        var transactions = ParseTransactionsReactive(stream).ToList();


        return transactions.Select(list =>
        {
            var grouped = list.GroupBy(t => t.SellOffer.ItemId);

            var stats = grouped.ToDictionary(g => g.Key, g => new TransactionStats
            {
                TotalCount = g.Sum(t => t.Count),
                AveragePrice = g.Average(t => t.SellOffer.Silver),
                MinPrice = g.Min(t => t.SellOffer.Silver),
                MaxPrice = g.Max(t => t.SellOffer.Silver),
                LastTransaction = g.Max(t => t.CreatedAt),
                ItemId = g.Key,
                TransactionCount = g.Count(),
                TotalVolume = g.Sum(t => t.Count * t.SellOffer.Silver),
                FirstTransaction = g.Min(t => t.CreatedAt),
                MedianPrice = g.Select(t => t.SellOffer.Silver).OrderBy(p => p).ElementAt(g.Count() / 2),
                StandardDeviation = g.Select(t => t.SellOffer.Silver)
                    .Average(p => Math.Pow(p - g.Average(t => t.SellOffer.Silver), 2)),
                TotalBought = 0,
                TotalSpent = 0,
                TotalSold = 0,
                TotalEarned = 0,
                ProfitLoss = 0,
            });

            return stats;
        }).Take(1);
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