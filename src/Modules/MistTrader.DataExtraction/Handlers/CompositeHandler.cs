namespace MistTrader.DataExtraction.Handlers;

using MediatR;
using MistTrader.DataExtraction.Errors;
using MistTrader.DataExtraction.Requests;
using OneOf;

internal class CompositeExtractObjectsHandler : IRequestHandler<ExtractObjectsFromResponse, OneOf<ExtractedData, ExtractionError>>
{
    private readonly IEnumerable<IResponseDataExtractor> _handlers;

    public CompositeExtractObjectsHandler(IEnumerable<IResponseDataExtractor> handlers)
    {
        _handlers = handlers;
    }

    public async Task<OneOf<ExtractedData, ExtractionError>> Handle(ExtractObjectsFromResponse request, CancellationToken cancellationToken)
    {
        var errors = ExtractionError.None;
        var extractedData = ExtractedData.Of();

        foreach (var handler in _handlers)
        {
            var result = await handler.Handle(request, cancellationToken);
            result.Switch(
                data => extractedData = extractedData.Merge(data),
                error => errors |= error
            );
        }

        if (errors != ExtractionError.None)
        {
            return errors;
        }
        
        return extractedData;
    }
}