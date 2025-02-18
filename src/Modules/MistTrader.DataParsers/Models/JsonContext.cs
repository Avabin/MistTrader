using System.Text.Json.Serialization;

namespace DataParsers.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Transaction))]
[JsonSerializable(typeof(BreederOffer))]
[JsonSerializable(typeof(TransactionStats))]
[JsonSerializable(typeof(List<Transaction>))]
[JsonSerializable(typeof(List<TransactionStats>))]
[JsonSerializable(typeof(InventoryItem))]
[JsonSerializable(typeof(List<InventoryItem>))]
public partial class JsonContext : JsonSerializerContext
{
}