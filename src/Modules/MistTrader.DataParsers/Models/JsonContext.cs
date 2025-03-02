using System.Text.Json.Serialization;

namespace DataParsers.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Transaction))]
[JsonSerializable(typeof(BreederOffer))]
[JsonSerializable(typeof(TransactionStats))]

[JsonSerializable(typeof(List<Transaction>))]
[JsonSerializable(typeof(Transaction[]))]
[JsonSerializable(typeof(List<TransactionStats>))]

[JsonSerializable(typeof(InventoryItem))]
[JsonSerializable(typeof(InventoryItem[]))]
[JsonSerializable(typeof(List<InventoryItem>))]

[JsonSerializable(typeof(Profile))]
[JsonSerializable(typeof(Role))]
[JsonSerializable(typeof(Role[]))]
[JsonSerializable(typeof(List<Role>))]
[JsonSerializable(typeof(List<Profile>))]
[JsonSerializable(typeof(DiscordConnection))]
[JsonSerializable(typeof(Achievement))]
[JsonSerializable(typeof(Achievement[]))]
[JsonSerializable(typeof(IReadOnlyList<Achievement>))]
[JsonSerializable(typeof(OnlineStatus))]
[JsonSerializable(typeof(LastVisit))]
[JsonSerializable(typeof(IReadOnlyList<LastVisit>))]
[JsonSerializable(typeof(Friendship))]
[JsonSerializable(typeof(Friendship[]))]
[JsonSerializable(typeof(IReadOnlyList<Friendship>))]
public partial class JsonContext : JsonSerializerContext
{
}