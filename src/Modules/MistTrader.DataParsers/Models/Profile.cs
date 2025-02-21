namespace DataParsers.Models;

public record Role
{
    public required string Type { get; init; }
}

public record Profile
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required bool HasUnfairAdvantage { get; init; }
    public string? AvatarUrl { get; init; }
    public string? PendingAvatarUrl { get; init; }
    public required string Gender { get; init; }
    public string? About { get; init; }
    public required long Silver { get; init; }
    public required int Rubies { get; init; }
    public required bool Newsletter { get; init; }
    public required bool HasFreeDragonLimitExpansion { get; init; }
    public required int BoughtDragonLimitExpansion { get; init; }
    public required int AdminDragonLimitExpansion { get; init; }
    public required int NextNameChangePrice { get; init; }
    public required int HieroglyphChangePrice { get; init; }
    public required DateTime LastHieroglyphChangePriceResetAt { get; init; }
    public required int Level { get; init; }
    public required int Xp { get; init; }
    public required int TotalXp { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int RemainingBattles { get; init; }
    public required DateTime LastRemainingBattlesResetAt { get; init; }
    public required int RemainingFeedXpGains { get; init; }
    public required DateTime LastRemainingFeedXpGainsResetAt { get; init; }
    public required IReadOnlyList<Role> Roles { get; init; }

    public static Profile CreateEmpty()
    {
        return new Profile
        {
            Id = 0,
            Name = "",
            HasUnfairAdvantage = false,
            Gender = "",
            Silver = 0,
            Rubies = 0,
            Newsletter = false,
            HasFreeDragonLimitExpansion = false,
            BoughtDragonLimitExpansion = 0,
            AdminDragonLimitExpansion = 0,
            NextNameChangePrice = 0,
            HieroglyphChangePrice = 0,
            LastHieroglyphChangePriceResetAt = default,
            Level = 0,
            Xp = 0,
            TotalXp = 0,
            CreatedAt = default,
            RemainingBattles = 0,
            LastRemainingBattlesResetAt = default,
            RemainingFeedXpGains = 0,
            LastRemainingFeedXpGainsResetAt = default,
            Roles = Array.Empty<Role>()
        };
    }
    
    public static bool IsEmpty(Profile profile) => profile.Id == 0;
}