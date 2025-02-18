using System.Text.Json;
using DataParsers;
using FluentAssertions;
using NUnit.Framework;

namespace MistTrader.DataParsers.Tests;

[TestFixture]
public class HighPerformanceParserTests
{
    private HighPerformanceParser _parser = null!;
    private string _sampleJson = null!;
    private byte[] _jsonBytes = null!;
    private string _tempFile = null!;

    [SetUp]
    public void Setup()
    {
        _parser = new HighPerformanceParser();
        
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

        _tempFile = Path.GetTempFileName();
        File.WriteAllText(_tempFile, _sampleJson);
    }

    [TearDown]
    public void Cleanup()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }

    [Test]
    public void ParseTransactions_WithValidJson_ShouldReturnTransactions()
    {
        // Act
        var result = _parser.ParseTransactions(_jsonBytes);

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
    public void ParseTransactions_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidJson = System.Text.Encoding.UTF8.GetBytes("{ invalid json }");

        // Act
        var result = _parser.ParseTransactions(invalidJson);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ParseFile_WithValidFile_ShouldReturnTransactions()
    {
        // Act
        var result = _parser.ParseFile(_tempFile);

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
    public void ParseFile_WithInvalidFile_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidFile = Path.GetTempFileName();
        File.WriteAllText(invalidFile, "{ invalid json }");

        // Act
        var result = _parser.ParseFile(invalidFile);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();

        // Cleanup
        File.Delete(invalidFile);
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
