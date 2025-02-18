using DataParsers;
using System.Reactive.Linq;
using DataParsers.Loaders;
using DataParsers.Parsers;

if (args.Length < 1)
{
    Console.WriteLine("Usage: MistTrader.Console <path-to-zip-file>");
    return;
}

var filePath = args[0];

if (!File.Exists(filePath))
{
    Console.WriteLine($"File not found: {filePath}");
    return;
}

IAsyncTradesParser parser = new AsyncTradesParser();
ZipLoader loader = new(parser);

try
{
    using var file = File.OpenRead(filePath);
    var transactions = await loader.LoadTransactionsFromZip(file);
    var stats = TradeStatsCalculator.CalculateStats(transactions);

    stats = stats.CalculatePersonalStats(transactions, 6305);
    

    if (!stats.Any())
    {
        Console.WriteLine("No transactions found in file");
        return;
    }

    var totalProfit = 0L;
    var items = stats.OrderByDescending(x => x.Value.TotalVolume);

    foreach (var (itemId, itemStats) in items)
    {
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"Item: {itemId}");
        Console.WriteLine($"Total Transactions: {itemStats.TransactionCount:N0}");
        Console.WriteLine($"Total Items Traded: {itemStats.TotalCount:N0}");
        Console.WriteLine($"Total Volume: {itemStats.TotalVolume:N0} silver");
        Console.WriteLine($"Average Price: {itemStats.AveragePrice:N2} silver");
        Console.WriteLine($"Price Range: {itemStats.MinPrice:N0} - {itemStats.MaxPrice:N0} silver");
        Console.WriteLine($"First Trade: {itemStats.FirstTransaction:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Last Trade: {itemStats.LastTransaction:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine();
        Console.WriteLine("Trading Summary:");
        Console.WriteLine($"Total Bought: {itemStats.TotalBought:N0} items");
        Console.WriteLine($"Total Spent: {itemStats.TotalSpent:N0} silver");
        Console.WriteLine($"Total Sold: {itemStats.TotalSold:N0} items");
        Console.WriteLine($"Total Earned: {itemStats.TotalEarned:N0} silver");
        Console.WriteLine($"Profit/Loss: {itemStats.ProfitLoss:N0} silver");
        Console.WriteLine("----------------------------------------");
        
        totalProfit += itemStats.ProfitLoss;
    }
    
    Console.WriteLine();
    Console.WriteLine($"Total Profit/Loss across all items: {totalProfit:N0} silver");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}