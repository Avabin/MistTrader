using System.Collections.Immutable;
using DataParsers.Models;
using MediatR;

namespace MistTrader.DataExtraction.Notifications;

public record InventoryDataExtracted(ImmutableList<InventoryItem> Inventory) : INotification
{
    
}