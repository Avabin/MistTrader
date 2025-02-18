using System.Text.Json.Serialization;

namespace DataParsers;

[JsonSourceGenerationOptions(
    WriteIndented = true
)]
[JsonSerializable(typeof(List<Transaction>))]
[JsonSerializable(typeof(Transaction))]
[JsonSerializable(typeof(BreederOffer))]
public partial class JsonContext : JsonSerializerContext
{
}