using System.Text.Json;
using MediatR;
using MistTrader.DataExtraction.Errors;
using MistTrader.DataExtraction.Requests;
using OneOf;

namespace MistTrader.DataExtraction.Handlers;
internal abstract class ExtractHandlerBase<T> : IResponseDataExtractor
{
    public abstract Task<OneOf<ExtractedData?, ExtractionError>> Handle(ExtractObjectsFromResponse request, CancellationToken cancellationToken);

    protected bool ContainsPathComponent(TrpcResponse response, string component)
    {
        var pathWithoutQuery = response.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        return pathWithoutQuery.Contains(component);
    }

    protected JsonElement GetJsonElement(TrpcResponse response)
    {
        return response.Json.GetProperty("result").GetProperty("data").GetProperty("json");
    }
}

internal interface IResponseDataExtractor

{
    Task<OneOf<ExtractedData?, ExtractionError>> Handle(ExtractObjectsFromResponse request, CancellationToken cancellationToken);
}