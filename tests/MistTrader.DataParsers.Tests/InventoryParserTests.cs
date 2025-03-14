using System.Text.Json;
using DataParsers;
using DataParsers.Models;
using DataParsers.Parsers;
using FluentAssertions;
using NUnit.Framework;

namespace MistTrader.DataParsers.Tests;

[TestFixture]
public class InventoryParserTests
{
    private string _sampleJson = null!;
    private string _tempFile = null!;

    [SetUp]
    public void Setup()
    {
        
        // Create sample test data
        var items = new[]
        {
            CreateInventoryItem("CrystalWater", 100, 1, false, false, 70),
            CreateInventoryItem("PandoraBox", 1, 1, true, false, 130000),
            CreateInventoryItem("SoulBoundItem", 1, 5, true, true, 1000),
        };

        _sampleJson = JsonSerializer.Serialize(items);
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
    public void ParseInventory_WithValidJson_ShouldReturnInventoryItems()
    {
        // Act
        var result = InventoryParser.ParseInventory(_sampleJson);

        // Assert
        result.Should().NotBeNull()
              .And.HaveCount(3);

        var firstItem = result.First();
        firstItem.Should().BeEquivalentTo(new InventoryItem(ItemId: "CrystalWater", Count: 100));
    }

    [Test]
    public void ParseInventory_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act
        var result = InventoryParser.ParseInventory(invalidJson);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ParseFile_WithValidFile_ShouldReturnInventoryItems()
    {
        // Act
        // var result = InventoryParser.ParseFile(_tempFile);
        //
        // // Assert
        // result.Should().NotBeNull()
        //       .And.HaveCount(3);
        //
        // result.Should().ContainEquivalentOf(new InventoryItem(ItemId: "PandoraBox", Count: 1));
    }

    [Test]
    public async Task ParseInventoryAsync_WithValidStream_ShouldReturnInventoryItems()
    {
        // Arrange
        using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(_sampleJson);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var result = await InventoryParser.ParseInventoryAsync(stream);

        // Assert
        result.Should().NotBeNull()
              .And.HaveCount(3);

        result.Should().ContainEquivalentOf(new InventoryItem(ItemId: "SoulBoundItem", Count: 1));
    }

    [Test]
    public async Task ParseFileAsync_WithValidFile_ShouldReturnInventoryItems()
    {
        // Act
        var result = await InventoryParser.ParseFileAsync(_tempFile);

        // Assert
        result.Should().NotBeNull()
              .And.HaveCount(3);

        var waterItem = result.FirstOrDefault(i => i.ItemId == "CrystalWater");
        waterItem.Should().NotBeNull();
        waterItem!.Count.Should().Be(100);
        waterItem.ItemId.Should().Be("CrystalWater");
    }

    [Test]
    public async Task ParseFileAsync_WithNonExistentFile_ShouldReturnEmptyList()
    {
        // Act
        var result = await InventoryParser.ParseFileAsync("nonexistent.json");

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }

    private static InventoryItem CreateInventoryItem(
        string itemId,
        int count,
        int level,
        bool isIdentified,
        bool soulBound,
        long silver)
    {
        return new InventoryItem(ItemId: itemId, Count: count);
    }
}