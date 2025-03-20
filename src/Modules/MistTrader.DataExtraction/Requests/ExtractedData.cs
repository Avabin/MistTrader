using System.Collections.Immutable;
using DataParsers.Models;
using DataParsers.Models.Messages;

namespace MistTrader.DataExtraction.Requests;

public record ExtractedData(
    Profile? Profile,
    ImmutableList<InventoryItem>? Inventory,
    ImmutableList<Transaction>? Transactions,
    ImmutableList<MessageModel>? Messages)
{
    public static ExtractedData Of(Profile? profile = null, ImmutableList<InventoryItem>? inventory = null, ImmutableList<Transaction>? transactions = null, ImmutableList<MessageModel>? messages = null)
    {
        return new ExtractedData(profile, inventory, transactions, messages);
    }

    public ExtractedData Merge(ExtractedData? data)
    {
        if (data == null)
            return this;
        return new ExtractedData(Profile: data.Profile ?? Profile, Inventory: data.Inventory ?? Inventory,
            Transactions: data.Transactions ?? Transactions, Messages: data.Messages ?? Messages);
    }
}