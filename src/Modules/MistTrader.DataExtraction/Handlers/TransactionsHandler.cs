using System.Collections.Immutable;
using System.Text;
using DataParsers;
using DataParsers.Models;
using MistTrader.DataExtraction.Errors;
using MistTrader.DataExtraction.Requests;
using OneOf;

namespace MistTrader.DataExtraction.Handlers;

internal class ExtractTransactionsHandler : ExtractHandlerBase<Transaction[]>
{
    private const string TransactionsMethodName = "exchange.transactionHistory";
    private readonly IAsyncTradesParser _tradesParser;

    public ExtractTransactionsHandler(IAsyncTradesParser tradesParser)
    {
        _tradesParser = tradesParser;
    }

    public override async Task<OneOf<ExtractedData?, ExtractionError>> Handle(ExtractObjectsFromResponse request, CancellationToken cancellationToken)
    {
        if (!ContainsPathComponent(request.Response, TransactionsMethodName))
            return null;

        try
        {
            var element = GetJsonElement(request.Response);
            var utf8Json = element.GetRawText();
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(utf8Json));
            var result = await _tradesParser.ParseTransactionsStreamAsync(stream);
            return ExtractedData.Of(transactions: result.ToImmutableList());
        }
        catch (Exception)
        {
            return ExtractionError.TransactionsError;
        }
    }
}