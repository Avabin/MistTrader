using System.Text.Json.Serialization;
using DataParsers.Models;

namespace MistTrader.DataExtraction;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TrpcResponse))]
internal partial class JsonContext : JsonSerializerContext
{
}