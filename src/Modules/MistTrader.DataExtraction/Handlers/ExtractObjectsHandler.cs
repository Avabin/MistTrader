using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using DataParsers;
using DataParsers.Models;
using DataParsers.Parsers;
using MediatR;
using MistTrader.DataExtraction.Errors;
using MistTrader.DataExtraction.Requests;
using OneOf;

namespace MistTrader.DataExtraction.Handlers;

internal class ExtractObjectsHandler : IRequestHandler<ExtractObjectsFromResponse, OneOf<ExtractedData, ExtractionError>>
{
    private readonly IAsyncTradesParser _tradesParser;

    public ExtractObjectsHandler(IAsyncTradesParser tradesParser)
    {
        _tradesParser = tradesParser;
    }
    private const string InventoryMethodName = "inventory.getItems";
    public async Task<OneOf<ExtractedData, ExtractionError>> Handle(ExtractObjectsFromResponse request,
        CancellationToken cancellationToken)
    {
        var response = request.Response;
        
        var errors = ExtractionError.None;
        
        InventoryItem[]? maybeInventory = null;
        try
        {
            maybeInventory = GetInventory(response);
        }
        catch (Exception e)
        {
            errors |= ExtractionError.CannotFindInventory;
        }
        
        Profile? maybeProfile = null;
        try
        {
            maybeProfile = GetProfile(response);
        }
        catch (Exception e)
        {
            errors |= ExtractionError.CannotFindProfile;
        }
        
        Transaction[]? maybeTransactions = null;
        try
        {
            maybeTransactions = await GetTransactions(response);
        }
        catch (Exception e)
        {
            errors |= ExtractionError.CannotFindTransactions;
        }

        if (errors != ExtractionError.None)
        {
            return errors;
        }
        
        return new ExtractedData(maybeProfile, maybeInventory?.ToImmutableList(), maybeTransactions?.ToImmutableList());
    }

    private InventoryItem[]? GetInventory(TrpcResponse response)
    {
        var url = response.Url;
        var pathWithoutQuery = url.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        var containsInventory = pathWithoutQuery.Contains(InventoryMethodName);
        if (!containsInventory)
        {
            return null;
        }
        
        
        var element = response.Json.GetProperty("result").GetProperty("data").GetProperty("json");
        var utf8Json = element.GetRawText();
        var result = InventoryParser.ParseInventory(utf8Json);
        
        return result.ToArray();
    }
    
    private Profile? GetProfile(TrpcResponse response)
    {
        var url = response.Url;
        var pathWithoutQuery = url.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        var containsProfile = pathWithoutQuery.Contains("breeders.getProfileDetails");
        if (!containsProfile)
        {
            return null;
        }

        var deserialized = ProfileParser.ParseJson(response.Json);
        
        return deserialized;
        
    }
    
    private async Task<Transaction[]?> GetTransactions(TrpcResponse response)
    {
        var url = response.Url;
        var pathWithoutQuery = url.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        var containsTransactions = pathWithoutQuery.Contains("exchange.transactionHistory");
        if (!containsTransactions)
        {
            return null;
        }

        // get result.json.data
        var element = response.Json.GetProperty("result").GetProperty("data").GetProperty("json");
        var utf8Json = element.GetRawText();
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(utf8Json));
        var result = await _tradesParser.ParseTransactionsStreamAsync(stream);
        
        return result.ToArray();
    }
}