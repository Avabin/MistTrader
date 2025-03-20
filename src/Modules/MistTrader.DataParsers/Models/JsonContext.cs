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

[JsonSerializable(typeof(Messages.MessagesData))]
[JsonSerializable(typeof(Messages.MessageModel))]
[JsonSerializable(typeof(Messages.MessageModel[]))]
[JsonSerializable(typeof(List<Messages.MessageModel>))]
[JsonSerializable(typeof(Messages.UserModel))]
[JsonSerializable(typeof(Messages.UserModel[]))]
[JsonSerializable(typeof(List<Messages.UserModel>))]
[JsonSerializable(typeof(Messages.DragonModel))]
[JsonSerializable(typeof(Messages.DragonModel[]))]
[JsonSerializable(typeof(List<Messages.DragonModel>))]
[JsonSerializable(typeof(Messages.AttachmentModel))]
[JsonSerializable(typeof(Messages.AttachmentModel[]))]
[JsonSerializable(typeof(List<Messages.AttachmentModel>))]
public partial class JsonContext : JsonSerializerContext
{
}