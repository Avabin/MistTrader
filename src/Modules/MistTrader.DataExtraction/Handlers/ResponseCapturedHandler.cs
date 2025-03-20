using System.Collections.Immutable;
using System.Text.Json;
using System.Xml;
using DataParsers.Models;
using MediatR;
using MistTrader.DataExtraction.Errors;
using MistTrader.DataExtraction.Notifications;
using MistTrader.DataExtraction.Requests;
using MistTrader.Proxy.Notifications;

namespace MistTrader.DataExtraction.Handlers;

internal class ResponseCapturedHandler : INotificationHandler<JsonResponseCaptured>
{
    private readonly IMediator _mediator;

    public ResponseCapturedHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Handle(JsonResponseCaptured notification, CancellationToken cancellationToken)
    {
        var url = notification.Url;
        var jsonResult = JsonSerializer.Deserialize<JsonDocument>(notification.Response);
        var trpcResponse = new TrpcResponse
        {
            Json = jsonResult?.RootElement ?? throw new ArgumentNullException(nameof(jsonResult)),
            Url = url
        };

        var request = new ExtractObjectsFromResponse(trpcResponse);
        var maybeData = await _mediator.Send(request, cancellationToken);
        
        if (maybeData.IsT0)
        {
            var data = maybeData.AsT0;
            var (profile, inventory, transactions, _) = data;
            
            await PublishProfileData(profile, cancellationToken);
            await PublishInventoryData(inventory, cancellationToken);
            await PublishTransactionData(transactions, cancellationToken);
        }
        else
        {
            var error = maybeData.AsT1;
            await _mediator.Publish(new ExtractionErrorOccured("Error during extraction", error), cancellationToken);
        }
    }
    
    private async Task PublishProfileData(Profile? profile, CancellationToken cancellationToken)
    {
        if (profile is null) return;
        var n = new ProfileDataExtracted(profile);
        await _mediator.Publish(n, cancellationToken);
    }
    
    private async Task PublishInventoryData(IReadOnlyList<InventoryItem>? inventory, CancellationToken cancellationToken)
    {
        if (inventory is null) return;
        var n = new InventoryDataExtracted(inventory.ToImmutableList());
        await _mediator.Publish(n, cancellationToken);
    }
    
    private async Task PublishTransactionData(IReadOnlyList<Transaction>? transactions, CancellationToken cancellationToken)
    {
        if (transactions is null) return;
        var n = new TransactionDataExtracted(transactions.ToImmutableList());
        await _mediator.Publish(n, cancellationToken);
    }
}