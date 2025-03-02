namespace DataParsers.Models;

/// <summary>
/// Represents a user's context in Mistwood, including their transaction history and current inventory
/// </summary>
public record MistwoodUserContext
{
    /// <summary>
    /// The breeder ID of the user
    /// </summary>
    public long BreederId => Profile.Id ?? 0;
    
    /// <summary>
    /// The username of the user
    /// </summary>
    public string Username => Profile.Name;
    
    /// <summary>
    /// The user's profile data
    /// </summary>
    public required Profile Profile { get; init; }
    
    /// <summary>
    /// List of all transactions associated with the user
    /// </summary>
    public required IReadOnlyList<Transaction> Transactions { get; init; }
    
    /// <summary>
    /// Current inventory state
    /// </summary>
    public required IReadOnlyList<InventoryItem> Inventory { get; init; }
    
    /// <summary>
    /// Last updated timestamp in UTC
    /// </summary>
    public required DateTime LastUpdated { get; init; }
    
    /// <summary>
    /// Calculate user's total trading statistics across all items
    /// </summary>
    public Dictionary<string, TransactionStats> CalculateTradeStats()
    {
        var marketStats = TradeStatsCalculator.CalculateStats(Transactions);
        return marketStats.CalculatePersonalStats(Transactions, BreederId);
    }
    
    /// <summary>
    /// Calculate total profit/loss across all trades
    /// </summary>
    public long CalculateTotalProfitLoss()
    {
        return CalculateTradeStats().Values.Sum(stats => stats.ProfitLoss);
    }
    
    /// <summary>
    /// Creates a new MistwoodUserContext with updated inventory
    /// </summary>
    public MistwoodUserContext WithInventory(IReadOnlyList<InventoryItem> newInventory)
    {
        return this with
        {
            Inventory = newInventory,
            LastUpdated = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a new MistwoodUserContext with added transactions
    /// </summary>
    public MistwoodUserContext WithNewTransactions(IReadOnlyList<Transaction> newTransactions)
    {
        var allTransactions = Transactions.Concat(newTransactions)
            .OrderBy(t => t.CreatedAt)
            .DistinctBy(x => x.Id)
            .ToList();
            
        return this with
        {
            Transactions = allTransactions,
            LastUpdated = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates an empty context for a user
    /// </summary>
    public static MistwoodUserContext CreateEmpty() =>
        new()
        {
            Transactions = Array.Empty<Transaction>(),
            Inventory = Array.Empty<InventoryItem>(),
            Profile = Profile.CreateEmpty(),
            LastUpdated = DateTime.UtcNow
        };
}