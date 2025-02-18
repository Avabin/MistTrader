using DataParsers.Models;

namespace DataParsers;

public interface ITradesParser
{
    IReadOnlyList<Transaction> ParseTransactions(ReadOnlySpan<byte> utf8Json);
    IReadOnlyList<Transaction> ParseFile(string filePath);
}

public interface IAsyncTradesParser
{
    Task<IReadOnlyList<Transaction>> ParseTransactionsStreamAsync(Stream stream);
    IAsyncEnumerable<Transaction> StreamTransactionsAsync(Stream stream, CancellationToken cancellationToken = default);
}

public interface IReactiveTradesParser
{
    IObservable<Transaction> ParseTransactionsReactive(Stream stream);
    IObservable<Dictionary<string, TransactionStats>> CalculateStatsReactive(Stream stream);
}