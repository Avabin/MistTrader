using System.Collections.Immutable;
using DataParsers.Models;
using DataParsers.Parsers;

namespace MistTrader.DataExtraction.Handlers;

using DataParsers;
using MediatR;
using MistTrader.DataExtraction.Errors;
using MistTrader.DataExtraction.Requests;
using OneOf;

internal class ExtractProfileHandler : ExtractHandlerBase<Profile>
{
    private const string ProfileMethodName = "breeders.getProfileDetails";

    public override async Task<OneOf<ExtractedData, ExtractionError>> Handle(ExtractObjectsFromResponse request, CancellationToken cancellationToken)
    {
        if (!ContainsPathComponent(request.Response, ProfileMethodName))
            return ExtractionError.ProfileError;

        try
        {
            var deserialized = ProfileParser.ParseJson(request.Response.Json);
            return ExtractedData.Of(profile: deserialized);
        }
        catch (Exception)
        {
            return ExtractionError.ProfileError;
        }
    }
}

internal class ExtractMessagesHandler : ExtractHandlerBase<Profile>
{
    private const string ProfileMethodName = "messages.getList";

    public override async Task<OneOf<ExtractedData?, ExtractionError>> Handle(ExtractObjectsFromResponse request, CancellationToken cancellationToken)
    {
        if (!ContainsPathComponent(request.Response, ProfileMethodName))
            return null;

        try
        {
            var deserialized = MessagesParser.ParseMessages(request.Response.Json);
            return ExtractedData.Of(messages: deserialized?.Messages.ToImmutableList());
        }
        catch (Exception)
        {
            return ExtractionError.MessagesError;
        }
    }
}