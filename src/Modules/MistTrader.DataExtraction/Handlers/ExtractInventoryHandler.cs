using System.Collections.Immutable;
using DataParsers.Models;
using DataParsers.Parsers;
using MistTrader.DataExtraction.Errors;
using MistTrader.DataExtraction.Requests;
using OneOf;

namespace MistTrader.DataExtraction.Handlers;

internal class ExtractInventoryHandler : ExtractHandlerBase<InventoryItem[]>
{
    private const string InventoryMethodName = "inventory.getItems";

    public override async Task<OneOf<ExtractedData?, ExtractionError>> Handle(ExtractObjectsFromResponse request, CancellationToken cancellationToken)
    {
        if (!ContainsPathComponent(request.Response, InventoryMethodName))
            return null;

        try
        {
            var element = GetJsonElement(request.Response);
            var utf8Json = element.GetRawText();
            var result = InventoryParser.ParseInventory(utf8Json).ToImmutableList();
            return ExtractedData.Of(inventory: result);
        }
        catch (Exception)
        {
            return ExtractionError.InventoryError;
        }
    }
}