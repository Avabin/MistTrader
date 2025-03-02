using System.Text.Json.Serialization;
using DataParsers.Models;

namespace MistTrader.DataExtraction;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TrpcResponse<Profile>))]
[JsonSerializable(typeof(TrpcResponse<Transaction[]>))]
[JsonSerializable(typeof(TrpcResponse<InventoryItem[]>))]
internal partial class JsonContext : JsonSerializerContext
{
}