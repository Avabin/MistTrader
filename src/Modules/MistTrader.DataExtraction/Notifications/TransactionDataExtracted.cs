using System.Collections.Immutable;
using DataParsers.Models;
using MediatR;

namespace MistTrader.DataExtraction.Notifications;

public record TransactionDataExtracted(ImmutableList<Transaction> Transactions) : INotification
{
    
}