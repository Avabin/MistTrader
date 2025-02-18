using System.IO.Compression;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using DataParsers.Models;
using DataParsers.Parsers;
using DataParsers.Loaders;

namespace MistTrader.DataParsers.Tests;

[TestFixture]
public class ZipLoaderTests
{
    private ZipLoader _loader = null!;
    private string _transactionsJson = null!;
    private string _inventoryJson = null!;
    private byte[] _zipBytes = null!;

    [SetUp]
    public void Setup()
    {
        _loader = new ZipLoader(new AsyncTradesParser());
        
        // Create sample test data
        var transactions = new[]
        {
            CreateTransaction(1, "CrystalWater", 100, 70, 10, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "CrystalWater", 100, 75, 5, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "PandoraBox", 130000, 130000, 1, "2025-02-17T20:15:00Z"),
        };

        var inventory = new[]
        {
            CreateInventoryItem("CrystalWater", 50, 1, false, false, 70),
            CreateInventoryItem("PandoraBox", 1, 1, true, false, 130000),
            CreateInventoryItem("SoulBoundItem", 1, 5, true, true, 1000),
        };

        _transactionsJson = JsonSerializer.Serialize(transactions);
        _inventoryJson = JsonSerializer.Serialize(inventory);

        // Create ZIP file in memory with both files
        using var memStream = new MemoryStream();
        using (var archive = new ZipArchive(memStream, ZipArchiveMode.Create, true))
        {
            var transactionsEntry = archive.CreateEntry("transactions.json");
            using (var writer = new StreamWriter(transactionsEntry.Open()))
            {
                writer.Write(_transactionsJson);
            }

            var inventoryEntry = archive.CreateEntry("inventory.json");
            using (var writer = new StreamWriter(inventoryEntry.Open()))
            {
                writer.Write(_inventoryJson);
            }
        }
        _zipBytes = memStream.ToArray();
    }

    [Test]
    public async Task LoadTransactionsFromZip_ShouldReturnAllTransactions()
    {
        // Arrange
        using var zipStream = new MemoryStream(_zipBytes);

        // Act
        var result = await _loader.LoadTransactionsFromZip(zipStream);

        // Assert
        result.Should().NotBeNull()
              .And.HaveCount(3)
              .And.BeInAscendingOrder(t => t.Id);

        var firstTransaction = result.First();
        firstTransaction.Should().BeEquivalentTo(new
        {
            Id = 1,
            SellOffer = new { ItemId = "CrystalWater", Silver = 70 },
            Count = 10,
            CreatedAt = DateTime.Parse("2025-02-17T20:00:00Z")
        }, options => options.ExcludingMissingMembers());
    }

    [Test]
    public async Task LoadInventoryFromZip_ShouldReturnAllInventoryItems()
    {
        // Arrange
        using var zipStream = new MemoryStream(_zipBytes);

        // Act
        var result = await _loader.LoadInventoryFromZip(zipStream);

        // Assert
        result.Should().NotBeNull()
              .And.HaveCount(3);

        result.Should().ContainEquivalentOf(new InventoryItem
        {
            ItemId = "CrystalWater",
            Count = 50,
            Level = 1,
            IsIdentified = false,
            SoulBound = false,
            Silver = 70
        });
    }

    [Test]
    public async Task LoadAllFromZip_ShouldReturnBothTransactionsAndInventory()
    {
        // Arrange
        using var zipStream = new MemoryStream(_zipBytes);

        // Act
        var (transactions, inventory) = await _loader.LoadAllFromZip(zipStream);

        // Assert
        transactions.Should().NotBeNull().And.HaveCount(3);
        inventory.Should().NotBeNull().And.HaveCount(3);

        transactions.Should().BeInAscendingOrder(t => t.Id);
        inventory.Should().Contain(i => i.ItemId == "SoulBoundItem" && i.SoulBound);
    }

    [Test]
    public async Task StreamTransactionsFromZip_ShouldStreamAllTransactions()
    {
        // Arrange
        using var zipStream = new MemoryStream(_zipBytes);
        var transactions = new List<Transaction>();

        // Act
        await foreach (var transaction in _loader.StreamTransactionsFromZip(zipStream))
        {
            transactions.Add(transaction);
        }

        // Assert
        transactions.Should().NotBeNull()
                  .And.HaveCount(3)
                  .And.BeInAscendingOrder(t => t.Id);

        transactions.First().SellOffer.ItemId.Should().Be("CrystalWater");
    }

    private static Transaction CreateTransaction(
        int id, 
        string itemId, 
        int buyPrice, 
        int sellPrice, 
        int count, 
        string createdAt)
    {
        return new Transaction
        {
            Id = id,
            Maker = "Sell",
            SellOffer = new BreederOffer
            {
                Id = id * 100,
                BreederId = id * 1000,
                ItemId = itemId,
                Silver = sellPrice
            },
            BuyOffer = new BreederOffer
            {
                Id = id * 100 + 1,
                BreederId = id * 1000 + 1,
                ItemId = itemId,
                Silver = buyPrice
            },
            Count = count,
            CreatedAt = DateTime.Parse(createdAt),
            Silver = sellPrice
        };
    }

    private static InventoryItem CreateInventoryItem(
        string itemId,
        int count,
        int level,
        bool isIdentified,
        bool soulBound,
        long silver)
    {
        return new InventoryItem
        {
            ItemId = itemId,
            Count = count,
            Level = level,
            IsIdentified = isIdentified,
            SoulBound = soulBound,
            Silver = silver
        };
    }
}