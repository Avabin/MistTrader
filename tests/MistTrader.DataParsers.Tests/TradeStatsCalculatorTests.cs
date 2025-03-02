using System;
using System.Text.Json;
using DataParsers;
using DataParsers.Models;
using DataParsers.Parsers;
using FluentAssertions;
using NUnit.Framework;

namespace MistTrader.DataParsers.Tests;

[TestFixture]
public class TradeStatsCalculatorTests
{
    private const long TestBreederId = 1000; // The breeder ID we'll use for personal stats
    private string _sampleJson = null!;
    private IAsyncTradesParser _parser = null!;

    [SetUp]
    public void Setup()
    {
        _parser = new AsyncTradesParser();

        // Create sample test data with known buyer/seller IDs
        var transactions = new[]
        {
            CreateTransaction(1, "CrystalWater", TestBreederId, 1001, "Buy", 70, 70, 10, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "CrystalWater", 1002, TestBreederId, "Sell", 75, 75, 5, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "PandoraBox", 1003, 1004, "Sell", 130000, 130000, 1, "2025-02-17T20:15:00Z"),
            CreateTransaction(4, "CrystalWater", TestBreederId, 1005, "Buy", 80, 80, 15, "2025-02-17T20:20:00Z")
        };

        _sampleJson = JsonSerializer.Serialize(transactions);
    }

    [Test]
    public async Task CalculateStats_ShouldReturnCorrectMarketStatistics()
    {
        // Arrange
        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            await writer.WriteAsync(_sampleJson);
            await writer.FlushAsync();
        }

        stream.Position = 0;

        // Act
        var transactions = await _parser.ParseTransactionsStreamAsync(stream);
        var stats = TradeStatsCalculator.CalculateStats(transactions);

        // Assert
        stats.Should().ContainKey("CrystalWater")
            .And.ContainKey("PandoraBox");

        var crystalWaterStats = stats["CrystalWater"];
        crystalWaterStats.Should().BeEquivalentTo(new TransactionStats
        {
            ItemId = "CrystalWater",
            TotalCount = 30, // 10 + 5 + 15
            TotalVolume = 70 * 10 + 75 * 5 + 80 * 15, // 700 + 375 + 1200 = 2275
            AveragePrice = (70 * 10 + 75 * 5 + 80 * 15) / 30.0,
            MinPrice = 70,
            MaxPrice = 80,
            TransactionCount = 3,
            FirstTransaction = DateTime.Parse("2025-02-17T20:00:00Z"),
            LastTransaction = DateTime.Parse("2025-02-17T20:20:00Z"),
            TotalBought = 0,
            TotalSpent = 0,
            TotalSold = 0,
            TotalEarned = 0,
            ProfitLoss = 0,
            MedianPrice = 75,
            StandardDeviation = 4.166666666666666,
            TotalTransactionValue = 2275
        });

        var pandoraBoxStats = stats["PandoraBox"];
        pandoraBoxStats.Should().BeEquivalentTo(new TransactionStats
        {
            ItemId = "PandoraBox",
            TotalCount = 1,
            TotalVolume = 130000,
            AveragePrice = 130000,
            MinPrice = 130000,
            MaxPrice = 130000,
            TransactionCount = 1,
            FirstTransaction = DateTime.Parse("2025-02-17T20:15:00Z"),
            LastTransaction = DateTime.Parse("2025-02-17T20:15:00Z"),
            TotalBought = 0,
            TotalSpent = 0,
            TotalSold = 0,
            TotalEarned = 0,
            ProfitLoss = 0,
            MedianPrice = 130000,
            StandardDeviation = 0,
            TotalTransactionValue = 130000
        });
    }

    [Test]
    public async Task CalculatePersonalStats_ShouldReturnCorrectPersonalStatistics()
    {
        // Arrange
        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            await writer.WriteAsync(_sampleJson);
            await writer.FlushAsync();
        }

        stream.Position = 0;

        // Act
        var transactions = await _parser.ParseTransactionsStreamAsync(stream);
        var marketStats = TradeStatsCalculator.CalculateStats(transactions);
        var personalStats = marketStats.CalculatePersonalStats(transactions, TestBreederId);

        // Assert
        personalStats.Should().ContainKey("CrystalWater")
            .And.ContainKey("PandoraBox");

        var crystalWaterStats = personalStats["CrystalWater"];
        crystalWaterStats.Should().BeEquivalentTo(new TransactionStats
        {
            ItemId = "CrystalWater",
            TotalCount = 30,
            TotalVolume = 70 * 10 + 75 * 5 + 80 * 15, // 2275
            AveragePrice = (70 * 10 + 75 * 5 + 80 * 15) / 30.0,
            MinPrice = 70,
            MaxPrice = 80,
            TransactionCount = 3,
            FirstTransaction = DateTime.Parse("2025-02-17T20:00:00Z"),
            LastTransaction = DateTime.Parse("2025-02-17T20:20:00Z"),
            TotalBought = 5, // One transaction where we are buyer (second transaction)
            TotalSpent = 375, // 5 * 75
            TotalSold = 25, // Two transactions where we are seller (first and fourth: 10 + 15)
            TotalEarned = 1900, // (10 * 70) + (15 * 80) = 700 + 1200
            ProfitLoss = 1525, // 1900 - 375
            MedianPrice = 75,
            StandardDeviation = 4.166666666666666,
            TotalTransactionValue = 2275
        });

        var pandoraBoxStats = personalStats["PandoraBox"];
        pandoraBoxStats.Should().BeEquivalentTo(new TransactionStats
        {
            ItemId = "PandoraBox",
            TotalCount = 1,
            TotalVolume = 130000,
            AveragePrice = 130000,
            MinPrice = 130000,
            MaxPrice = 130000,
            TransactionCount = 1,
            FirstTransaction = DateTime.Parse("2025-02-17T20:15:00Z"),
            LastTransaction = DateTime.Parse("2025-02-17T20:15:00Z"),
            TotalBought = 0, // We're not involved in PandoraBox trades
            TotalSpent = 0,
            TotalSold = 0,
            TotalEarned = 0,
            ProfitLoss = 0,
            MedianPrice = 130000,
            StandardDeviation = 0,
            TotalTransactionValue = 130000
        });
    }

    [Test]
    public void CalculateStats_WithEmptyTransactions_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var emptyTransactions = Array.Empty<Transaction>();

        // Act
        var stats = TradeStatsCalculator.CalculateStats(emptyTransactions);

        // Assert
        stats.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void CalculatePersonalStats_WithEmptyTransactions_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var emptyTransactions = Array.Empty<Transaction>();
        var emptyMarketStats = TradeStatsCalculator.CalculateStats(emptyTransactions);

        // Act
        var personalStats = emptyMarketStats.CalculatePersonalStats(emptyTransactions, TestBreederId);

        // Assert
        personalStats.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public async Task CalculatePersonalStats_WithNoPersonalTransactions_ShouldReturnZeroPersonalStats()
    {
        // Arrange
        var transactions = new[]
        {
            CreateTransaction(1, "TestItem", 1001, 1002, "Sell", 100, 100, 1, "2025-02-17T20:00:00Z")
        };
        var json = JsonSerializer.Serialize(transactions);

        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            await writer.WriteAsync(json);
            await writer.FlushAsync();
        }

        stream.Position = 0;

        // Act
        var parsedTransactions = await _parser.ParseTransactionsStreamAsync(stream);
        var marketStats = TradeStatsCalculator.CalculateStats(parsedTransactions);
        var personalStats = marketStats.CalculatePersonalStats(parsedTransactions, TestBreederId);

        // Assert
        var itemStats = personalStats["TestItem"];
        itemStats.TotalBought.Should().Be(0);
        itemStats.TotalSpent.Should().Be(0);
        itemStats.TotalSold.Should().Be(0);
        itemStats.TotalEarned.Should().Be(0);
        itemStats.ProfitLoss.Should().Be(0);
    }

    [Test]
    public async Task CalculateStats_ShouldCalculateMedianPrice()
    {
        // Arrange
        var transactions = new[]
        {
            CreateTransaction(1, "TestItem", 1001, 1002, "Sell", 100, 100, 1, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "TestItem", 1003, 1004, "Sell", 200, 200, 1, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "TestItem", 1005, 1006, "Sell", 300, 300, 1, "2025-02-17T20:20:00Z")
        };
        var json = JsonSerializer.Serialize(transactions);

        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            await writer.WriteAsync(json);
            await writer.FlushAsync();
        }

        stream.Position = 0;

        // Act
        var parsedTransactions = await _parser.ParseTransactionsStreamAsync(stream);
        var stats = TradeStatsCalculator.CalculateStats(parsedTransactions);

        // Assert
        var itemStats = stats["TestItem"];
        itemStats.MedianPrice.Should().Be(200);
    }

    [Test]
    public async Task CalculateStats_ShouldCalculateStandardDeviation()
    {
        // Arrange
        var transactions = new[]
        {
            CreateTransaction(1, "TestItem", 1001, 1002, "Sell", 100, 100, 1, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "TestItem", 1003, 1004, "Sell", 200, 200, 1, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "TestItem", 1005, 1006, "Sell", 300, 300, 1, "2025-02-17T20:20:00Z")
        };
        var json = JsonSerializer.Serialize(transactions);

        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            await writer.WriteAsync(json);
            await writer.FlushAsync();
        }

        stream.Position = 0;

        // Act
        var parsedTransactions = await _parser.ParseTransactionsStreamAsync(stream);
        var stats = TradeStatsCalculator.CalculateStats(parsedTransactions);

        // Assert
        var itemStats = stats["TestItem"];
        itemStats.StandardDeviation.Should().BeApproximately(81.65, 0.01);
    }

    [Test]
    public async Task CalculateStats_ShouldCalculateTotalTransactionValue()
    {
        // Arrange
        var transactions = new[]
        {
            CreateTransaction(1, "TestItem", 1001, 1002, "Sell", 100, 100, 1, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "TestItem", 1003, 1004, "Sell", 200, 200, 1, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "TestItem", 1005, 1006, "Sell", 300, 300, 1, "2025-02-17T20:20:00Z")
        };
        var json = JsonSerializer.Serialize(transactions);

        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            await writer.WriteAsync(json);
            await writer.FlushAsync();
        }

        stream.Position = 0;

        // Act
        var parsedTransactions = await _parser.ParseTransactionsStreamAsync(stream);
        var stats = TradeStatsCalculator.CalculateStats(parsedTransactions);

        // Assert
        var itemStats = stats["TestItem"];
        itemStats.TotalTransactionValue.Should().Be(600);
    }

    [Test]
    public void CalculateInventoryStats_ShouldCalculateCorrectStatistics()
    {
        // Arrange
        var inventoryItems = new List<InventoryItemDetails>
        {
            new InventoryItemDetails { ItemId = "Item1", Count = 10, Level = 1, IsIdentified = true, SoulBound = false, Silver = 100 },
            new InventoryItemDetails { ItemId = "Item2", Count = 5, Level = 2, IsIdentified = true, SoulBound = false, Silver = 200 },
            new InventoryItemDetails { ItemId = "Item3", Count = 15, Level = 3, IsIdentified = true, SoulBound = false, Silver = 50 }
        };

        // Act
        var inventoryStats = TradeStatsCalculator.CalculateInventoryStats(inventoryItems);

        // Assert
        inventoryStats.TotalValue.Should().Be(10 * 100 + 5 * 200 + 15 * 50); // 1000 + 1000 + 750 = 2750
        inventoryStats.TotalCount.Should().Be(10 + 5 + 15); // 30
        inventoryStats.ItemCount.Should().Be(3);
        inventoryStats.AverageValue.Should().Be(2750 / 3.0); // 916.67
    }

    private static Transaction CreateTransaction(
        int id,
        string itemId,
        long sellerBreederId,
        long buyerBreederId,
        string maker,
        int buyPrice,
        int sellPrice,
        int count,
        string createdAt)
    {
        return new Transaction
        {
            Id = id,
            Maker = maker,
            SellOffer = new BreederOffer
            {
                Id = id * 100,
                BreederId = sellerBreederId,
                ItemId = itemId,
                Silver = sellPrice
            },
            BuyOffer = new BreederOffer
            {
                Id = id * 100 + 1,
                BreederId = buyerBreederId,
                ItemId = itemId,
                Silver = buyPrice
            },
            Count = count,
            CreatedAt = DateTime.Parse(createdAt),
            Silver = sellPrice
        };
    }
}
