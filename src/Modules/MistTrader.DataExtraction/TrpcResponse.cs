using System.Text.Json;
using DataParsers.Models;
using MistTrader.Proxy.Notifications;

namespace MistTrader.DataExtraction;

internal record TrpcResponse
{
    public required JsonElement Json { get; init; }
    public required Uri Url { get; init; }
}
