
using System.Text.Json;

namespace DataParsers;

public class ExchangeFileParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<IReadOnlyList<Transaction>> ParseTransactionsFileAsync(string filePath)
    {
        await using var fileStream = File.OpenRead(filePath);
        return await ParseTransactionsStreamAsync(fileStream);
    }

    public async Task<IReadOnlyList<Transaction>> ParseTransactionsStreamAsync(Stream jsonStream)
    {
        try
        {
            var transactions = await JsonSerializer.DeserializeAsync<Transaction[]>(
                jsonStream, 
                Options);
                
            return transactions ?? Array.Empty<Transaction>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing transactions file: {ex.Message}");
            return Array.Empty<Transaction>();
        }
    }
    
    public async Task<IReadOnlyDictionary<string, TransactionStats>> CalculateStatsAsync(string filePath)
    {
        var transactions = await ParseTransactionsFileAsync(filePath);
        return CalculateStats(transactions);
    }
    
    private static IReadOnlyDictionary<string, TransactionStats> CalculateStats(IEnumerable<Transaction> transactions)
    {
        return transactions
            .GroupBy(t => t.SellOffer.ItemId)
            .ToDictionary(
                g => g.Key,
                g => new TransactionStats
                {
                    ItemId = g.Key,
                    TotalCount = g.Sum(t => t.Count),
                    TotalVolume = g.Sum(t => t.Count * t.Silver),
                    AveragePrice = g.Average(t => t.Silver),
                    MinPrice = g.Min(t => t.Silver),
                    MaxPrice = g.Max(t => t.Silver),
                    TransactionCount = g.Count(),
                    FirstTransaction = g.Min(t => t.CreatedAt),
                    LastTransaction = g.Max(t => t.CreatedAt)
                });
    }
}