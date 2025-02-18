using System.Text.Json;
using DataParsers;
using DataParsers.Models;
using DataParsers.Parsers;
using FluentAssertions;
using NUnit.Framework;

namespace MistTrader.DataParsers.Tests;

[TestFixture]
public class HighPerformanceJsonTradesParserTests
{
    private string _sampleJson = null!;
    private byte[] _jsonBytes = null!;

    [SetUp]
    public void Setup()
    {
        // Create sample test data
        var transactions = new[]
        {
            CreateTransaction(1, "CrystalWater", 100, 70, 10, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "CrystalWater", 100, 75, 5, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "PandoraBox", 130000, 130000, 1, "2025-02-17T20:15:00Z"),
            CreateTransaction(4, "CrystalWater", 100, 80, 15, "2025-02-17T20:20:00Z")
        };

        _sampleJson = JsonSerializer.Serialize(transactions);
        _jsonBytes = System.Text.Encoding.UTF8.GetBytes(_sampleJson);
    }

    [Test]
    public void GetEnumerator_WithValidJson_ShouldReturnTransactions()
    {
        // Arrange
        var parser = new HighPerformanceJsonTradesParser(_jsonBytes);

        // Act
        var result = parser.ToList();

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
    public void GetEnumerator_WithInvalidJson_ShouldThrowJsonException()
    {
        // Arrange
        var invalidJson = "{ invalid json }"u8.ToArray();

        // Act
        var act = new Action(() => new HighPerformanceJsonTradesParser(invalidJson).ToList());

        // Assert
        act.Should().Throw<JsonException>();
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
