using System.IO.Compression;
using System.Text.Json;
using DataParsers.Models;

namespace DataParsers.Loaders;

public class ZipLoader
{
    private readonly IAsyncTradesParser _tradesParser;
    private readonly JsonSerializerOptions _options;

    public ZipLoader(IAsyncTradesParser tradesParser)
    {
        _tradesParser = tradesParser;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = JsonContext.Default
        };
    }

    /// <summary>
    /// Streams transactions from a ZIP file containing transactions.json
    /// </summary>
    public async IAsyncEnumerable<Transaction> StreamTransactionsFromZip(Stream zipStream)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var transactionsEntry = archive.GetEntry("transactions.json");
        
        if (transactionsEntry == null)
        {
            yield break;
        }

        using var transactionStream = transactionsEntry.Open();
        await foreach (var transaction in _tradesParser.StreamTransactionsAsync(transactionStream))
        {
            yield return transaction;
        }
    }

    /// <summary>
    /// Loads all transactions from ZIP file at once
    /// </summary>
    public async Task<IReadOnlyList<Transaction>> LoadTransactionsFromZip(Stream zipStream)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var transactionsEntry = archive.GetEntry("transactions.json");
        
        if (transactionsEntry == null)
        {
            return Array.Empty<Transaction>();
        }

        using var transactionStream = transactionsEntry.Open();
        return await _tradesParser.ParseTransactionsStreamAsync(transactionStream);
    }
}