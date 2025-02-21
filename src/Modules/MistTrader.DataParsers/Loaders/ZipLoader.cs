using System.IO.Compression;
using System.Text.Json;
using DataParsers.Models;
using DataParsers.Parsers;

namespace DataParsers.Loaders;

public class ZipLoader
{
    private readonly IAsyncTradesParser _tradesParser;
    private readonly JsonSerializerOptions _options;

    // Expected file names in the ZIP
    private const string TransactionsFileName = "transactions.json";
    private const string InventoryFileName = "inventory.json";
    private const string ProfileFileName = "profile.json";

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
        var transactionsEntry = archive.GetEntry(TransactionsFileName);

        if (transactionsEntry == null)
        {
            yield break;
        }

        await using var transactionStream = transactionsEntry.Open();
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
        var transactionsEntry = archive.GetEntry(TransactionsFileName);

        if (transactionsEntry == null)
        {
            return Array.Empty<Transaction>();
        }

        await using var transactionStream = transactionsEntry.Open();
        return await _tradesParser.ParseTransactionsStreamAsync(transactionStream);
    }

    /// <summary>
    /// Loads inventory items from ZIP file
    /// </summary>
    public async Task<IReadOnlyList<InventoryItem>> LoadInventoryFromZip(Stream zipStream)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var inventoryEntry = archive.GetEntry(InventoryFileName);

        if (inventoryEntry == null)
        {
            return Array.Empty<InventoryItem>();
        }

        await using var inventoryStream = inventoryEntry.Open();
        return await JsonSerializer.DeserializeAsync<List<InventoryItem>>(inventoryStream, _options)
               ?? [];
    }

    /// <summary>
    /// Loads profile data from ZIP file
    /// </summary>
    public async Task<Profile?> LoadProfileFromZip(Stream zipStream)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var profileEntry = archive.GetEntry(ProfileFileName);

        if (profileEntry == null)
        {
            return null;
        }

        await using var profileStream = profileEntry.Open();
        return await JsonSerializer.DeserializeAsync<Profile>(profileStream, _options);
    }

    /// <summary>
    /// Loads all data from a ZIP file including profile
    /// </summary>
    public async
        Task<(IReadOnlyList<Transaction> Transactions, IReadOnlyList<InventoryItem> Inventory, Profile? Profile)>
        LoadAllFromZip(Stream zipStream)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        // Create tasks for parallel processing
        var transactionTask = LoadTransactionsFromEntry(archive.GetEntry(TransactionsFileName));
        var inventoryTask = LoadInventoryFromEntry(archive.GetEntry(InventoryFileName));
        var profileTask = LoadProfileFromEntry(archive.GetEntry(ProfileFileName));

        // Wait for all tasks to complete
        await Task.WhenAll(transactionTask, inventoryTask, profileTask);

        return (await transactionTask, await inventoryTask, await profileTask);
    }

    private async Task<Profile?> LoadProfileFromEntry(ZipArchiveEntry? entry)
    {
        if (entry == null) return null;

        await using var stream = entry.Open();
        return await JsonSerializer.DeserializeAsync<Profile>(stream, _options);
    }

    private async Task<IReadOnlyList<Transaction>> LoadTransactionsFromEntry(ZipArchiveEntry? entry)
    {
        if (entry == null) return Array.Empty<Transaction>();

        await using var stream = entry.Open();
        return await _tradesParser.ParseTransactionsStreamAsync(stream);
    }

    private async Task<IReadOnlyList<InventoryItem>> LoadInventoryFromEntry(ZipArchiveEntry? entry)
    {
        if (entry == null) return Array.Empty<InventoryItem>();

        await using var stream = entry.Open();
        return await JsonSerializer.DeserializeAsync<List<InventoryItem>>(stream, _options)
               ?? [];
    }
}