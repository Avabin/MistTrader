using System.Text.Json.Serialization;

namespace DataParsers.Models;


// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record Achievement(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("breederId")] long? BreederId,
        [property: JsonPropertyName("achievedAt")] DateTimeOffset? AchievedAt
    );

    public record DiscordConnection(
        [property: JsonPropertyName("snowflake")] string Snowflake,
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("globalName")] string GlobalName,
        [property: JsonPropertyName("avatar")] string Avatar,
        [property: JsonPropertyName("mistwoodNick")] string MistwoodNick,
        [property: JsonPropertyName("mistwoodAvatar")] object MistwoodAvatar,
        [property: JsonPropertyName("mistwoodJoinedAt")] DateTimeOffset? MistwoodJoinedAt,
        [property: JsonPropertyName("mistwoodPremiumSince")] object MistwoodPremiumSince
    );

    public record Dragon(
        [property: JsonPropertyName("id")] long? Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("element")] string Element,
        [property: JsonPropertyName("appearanceVariant")] int? AppearanceVariant,
        [property: JsonPropertyName("level")] int? Level,
        [property: JsonPropertyName("energy")] int? Energy,
        [property: JsonPropertyName("gender")] string Gender,
        [property: JsonPropertyName("elo")] int? Elo,
        [property: JsonPropertyName("owner")] Owner Owner
    );

    public record Friend(
        [property: JsonPropertyName("id")] long? Id,
        [property: JsonPropertyName("roles")] IReadOnlyList<Role> Roles,
        [property: JsonPropertyName("avatarUrl")] string AvatarUrl,
        [property: JsonPropertyName("level")] int? Level,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("gender")] string Gender
    );

    public record Friendship(
        [property: JsonPropertyName("friend")] Friend Friend,
        [property: JsonPropertyName("startedAt")] DateTimeOffset? StartedAt
    );

    public record LastVisit(
        [property: JsonPropertyName("visitor")] Visitor Visitor,
        [property: JsonPropertyName("lastVisitAt")] DateTimeOffset? LastVisitAt
    );

    public record OnlineStatus(
        [property: JsonPropertyName("lastSeenAt")] DateTimeOffset? LastSeenAt
    );

    public record Owner(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("id")] long? Id,
        [property: JsonPropertyName("roles")] IReadOnlyList<Role> Roles,
        [property: JsonPropertyName("gender")] string Gender
    );

    public record Role(
        [property: JsonPropertyName("type")] string Type
    );

    public record Profile(
        [property: JsonPropertyName("id")] long? Id,
        [property: JsonPropertyName("roles")] IReadOnlyList<Role> Roles,
        [property: JsonPropertyName("avatarUrl")]
        object AvatarUrl,
        [property: JsonPropertyName("level")] int? Level,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("gender")] string Gender,
        [property: JsonPropertyName("discordConnection")]
        DiscordConnection DiscordConnection,
        [property: JsonPropertyName("about")] object About,
        [property: JsonPropertyName("silver")] int? Silver,
        [property: JsonPropertyName("rubies")] int? Rubies,
        [property: JsonPropertyName("achievements")]
        IReadOnlyList<Achievement> Achievements,
        [property: JsonPropertyName("createdAt")]
        DateTimeOffset? CreatedAt,
        [property: JsonPropertyName("dragons")]
        IReadOnlyList<Dragon> Dragons,
        [property: JsonPropertyName("onlineStatus")]
        OnlineStatus OnlineStatus,
        [property: JsonPropertyName("uniqueVisitorsCount")]
        int? UniqueVisitorsCount,
        [property: JsonPropertyName("lastVisits")]
        IReadOnlyList<LastVisit> LastVisits,
        [property: JsonPropertyName("friendships")]
        IReadOnlyList<Friendship> Friendships,
        [property: JsonPropertyName("banned")] bool? Banned
    )
    {
        public Profile() : this(0, new List<Role>(), new object(), 0, "", "", new DiscordConnection("", "", "", "", "", new object(), null, new object()), new object(), 0, 0, new List<Achievement>(), null, new List<Dragon>(), new OnlineStatus(null), 0, new List<LastVisit>(), new List<Friendship>(), false)
        {
            
        }
        public static Profile CreateEmpty() => new();
    }

    public record Visitor(
        [property: JsonPropertyName("id")] int? Id,
        [property: JsonPropertyName("roles")] IReadOnlyList<object> Roles,
        [property: JsonPropertyName("avatarUrl")] string AvatarUrl,
        [property: JsonPropertyName("level")] int? Level,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("gender")] string Gender
    );

