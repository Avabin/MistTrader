using DataParsers.Models;
using FluentAssertions;
using NUnit.Framework;

namespace MistTrader.DataParsers.Tests;

[TestFixture]
public class MistwoodUserContextTests
{
    private MistwoodUserContext _context = null!;
    private readonly long _testBreederId = 6305;
    private readonly string _testUsername = "TestUser";
    
    [SetUp]
    public void Setup()
    {
        // Create sample transactions
        var transactions = new[]
        {
            CreateTransaction(1, "CrystalWater", _testBreederId, 1001, 70, 70, 10, "2025-02-17T20:00:00Z"),
            CreateTransaction(2, "CrystalWater", 1002, _testBreederId, 75, 75, 5, "2025-02-17T20:10:00Z"),
            CreateTransaction(3, "PandoraBox", 1003, 1004, 130000, 130000, 1, "2025-02-17T20:15:00Z"),
        };
        
        // Create sample inventory
        var inventory = new[]
        {
            CreateInventoryItem("CrystalWater", 50, 1, false, false, 70),
            CreateInventoryItem("PandoraBox", 1, 1, true, false, 130000),
        };
        
        _context = new MistwoodUserContext
        {
            BreederId = _testBreederId,
            Username = _testUsername,
            Transactions = transactions,
            Inventory = inventory,
            LastUpdated = DateTime.UtcNow
        };
    }
    
    [Test]
    public void CalculateTradeStats_ShouldCalculateCorrectStats()
    {
        // Act
        var stats = _context.CalculateTradeStats();
        
        // Assert
        stats.Should().ContainKey("CrystalWater");
        
        var crystalWaterStats = stats["CrystalWater"];
        crystalWaterStats.TotalBought.Should().Be(5); // Bought 5
        crystalWaterStats.TotalSold.Should().Be(10);  // Sold 10
        crystalWaterStats.TotalSpent.Should().Be(375); // 5 * 75
        crystalWaterStats.TotalEarned.Should().Be(700); // 10 * 70
        crystalWaterStats.ProfitLoss.Should().Be(325); // 700 - 375
    }
    
    [Test]
    public void CalculateTotalProfitLoss_ShouldSumAllProfitsAndLosses()
    {
        // Act
        var totalProfitLoss = _context.CalculateTotalProfitLoss();
        
        // Assert
        totalProfitLoss.Should().Be(325); // Only profit from CrystalWater trades
    }
    
    [Test]
    public void CalculateInventoryValue_ShouldSumAllItemValues()
    {
        // Act
        var totalValue = _context.CalculateInventoryValue();
        
        // Assert
        var expectedValue = (50 * 70) + (1 * 130000); // CrystalWater + PandoraBox
        totalValue.Should().Be(expectedValue);
    }
    
    [Test]
    public void WithInventory_ShouldCreateNewContextWithUpdatedInventory()
    {
        // Arrange
        var newInventory = new[]
        {
            CreateInventoryItem("CrystalWater", 60, 1, false, false, 70),
        };
        
        // Act
        var newContext = _context.WithInventory(newInventory);
        
        // Assert
        newContext.Should().NotBeSameAs(_context);
        newContext.Inventory.Should().HaveCount(1);
        newContext.Inventory.First().Count.Should().Be(60);
        newContext.LastUpdated.Should().BeAfter(_context.LastUpdated);
    }
    
    [Test]
    public void WithNewTransactions_ShouldAddTransactionsInChronologicalOrder()
    {
        // Arrange
        var newTransactions = new[]
        {
            CreateTransaction(4, "CrystalWater", _testBreederId, 1005, 80, 80, 15, "2025-02-17T20:20:00Z"),
        };
        
        // Act
        var newContext = _context.WithNewTransactions(newTransactions);
        
        // Assert
        newContext.Should().NotBeSameAs(_context);
        newContext.Transactions.Should().HaveCount(4);
        newContext.Transactions.Last().Id.Should().Be(4);
        newContext.LastUpdated.Should().BeAfter(_context.LastUpdated);
    }
    
    [Test]
    public void CreateEmpty_ShouldCreateEmptyContext()
    {
        // Act
        var emptyContext = MistwoodUserContext.CreateEmpty(_testBreederId, _testUsername);
        
        // Assert
        emptyContext.BreederId.Should().Be(_testBreederId);
        emptyContext.Username.Should().Be(_testUsername);
        emptyContext.Transactions.Should().BeEmpty();
        emptyContext.Inventory.Should().BeEmpty();
        emptyContext.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    private static Transaction CreateTransaction(
        int id,
        string itemId,
        long sellerBreederId,
        long buyerBreederId,
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