using System.Text.Json.Serialization;

namespace DataParsers.Models.Messages;

public record MessagesData(
    [property: JsonPropertyName("cursor")]
    int Cursor,
    [property: JsonPropertyName("messages")]
    IReadOnlyList<MessageModel> Messages
);

public record MessageModel(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("folder")]
    string Folder,
    [property: JsonPropertyName("subject")]
    string Subject,
    // not a typo, that's how it's spelled in the JSON
    [property: JsonPropertyName("attachementsClaimedAt")]
    DateTime AttachmentsClaimedAt,
    [property: JsonPropertyName("createdAt")]
    DateTime CreatedAt,
    [property: JsonPropertyName("expeditionLocation")]
    string ExpeditionLocation,
    [property: JsonPropertyName("expeditionEffects")]
    IReadOnlyList<string> ExpeditionEffects,
    [property: JsonPropertyName("expeditionWeather")]
    string ExpeditionWeather,
    [property: JsonPropertyName("expeditionDuration")]
    int ExpeditionDuration,
    [property: JsonPropertyName("dragon")]
    DragonModel Dragon,
    [property: JsonPropertyName("recipient")]
    UserModel Recipient,
    [property: JsonPropertyName("sender")]
    UserModel? Sender,
    [property: JsonPropertyName("content")]
    string Content,
    [property: JsonPropertyName("attachedSilver")]
    int AttachedSilver,
    [property: JsonPropertyName("attachedRubies")]
    int AttachedRubies,
    // not a typo, that's how it's spelled in the JSON
    [property: JsonPropertyName("attachements")]
    IReadOnlyList<AttachmentModel> Attachments
);

public record DragonModel(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("gender")]
    string Gender
);

public record AttachmentModel(
    [property: JsonPropertyName("itemId")]
    string ItemId,
    [property: JsonPropertyName("count")]
    int Count
);

public record UserModel(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("gender")]
    string Gender,
    [property: JsonPropertyName("level")]
    int Level,
    [property: JsonPropertyName("avatarUrl")]
    string? AvatarUrl,
    [property: JsonPropertyName("roles")]
    IReadOnlyList<string> Roles
);