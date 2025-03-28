using System.Text.Json;
using DataParsers;
using DataParsers.Models;
using DataParsers.Parsers;
using FluentAssertions;
using NUnit.Framework;

namespace MistTrader.DataParsers.Tests;

[TestFixture]
public class AsyncTradesParserTests
{
    private AsyncTradesParser _parser = null!;
    private string _sampleJson = null!;

    [SetUp]
    public void Setup()
    {
        _parser = new AsyncTradesParser();
        
        // Create sample test data
        var transactions = new[]
        {
            CreateTransaction(1, "CrystalWater", 100, 70, 10, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "CrystalWater", 100, 75, 5, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "PandoraBox", 130000, 130000, 1, "2025-02-17T20:15:00Z"),
            CreateTransaction(4, "CrystalWater", 100, 80, 15, "2025-02-17T20:20:00Z")
        };

        _sampleJson = JsonSerializer.Serialize(transactions);
    }

    [Test]
    public async Task ParseTransactionsStreamAsync_WithValidJson_ShouldReturnTransactions()
    {
        // Arrange
        using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(_sampleJson);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var result = await _parser.ParseTransactionsStreamAsync(stream);

        // Assert
        result.Should().NotBeNull()
              .And.HaveCount(4)
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
    public async Task ParseTransactionsStreamAsync_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidJson = "{ invalid json }";
        using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(invalidJson);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var result = await _parser.ParseTransactionsStreamAsync(stream);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public async Task CalculateStats_ShouldReturnCorrectStatistics()
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
            StandardDeviation = 4.166666666666666
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
            StandardDeviation = 0
        });
    }

    [Test]
    public async Task ParseTransactionsStreamAsync_WithEmptyStream_ShouldReturnEmptyList()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var result = await _parser.ParseTransactionsStreamAsync(stream);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
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
}