using MediatR;
using MistTrader.DataExtraction.Errors;
using OneOf;

namespace MistTrader.DataExtraction.Requests;

internal record ExtractObjectsFromResponse(TrpcResponse Response) : IRequest<OneOf<ExtractedData, ExtractionError>>
{
    
}