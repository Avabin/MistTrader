using System.Collections.Immutable;
using DataParsers.Models;

namespace MistTrader.DataExtraction.Requests;

public record ExtractedData(Profile? Profile, ImmutableList<InventoryItem>? Inventory, ImmutableList<Transaction>? Transactions);