namespace Rogue.Domain;

public static class Constants
{
    // Level Generation
    public const int RoomsInWidth = 3;
    public const int RoomsInHeight = 3;
    public const int RoomsNum = RoomsInWidth * RoomsInHeight;
    public const int RegionWidth = 27;
    public const int RegionHeight = 10;
    public const int MinRoomWidth = 6;
    public const int MaxRoomWidth = RegionWidth - 2;
    public const int MinRoomHeight = 5;
    public const int MaxRoomHeight = RegionHeight - 2;
    public const int MapWidth = RoomsInWidth * RegionWidth;
    public const int MapHeight = RoomsInHeight * RegionHeight;
    public const int MaxLevels = 21;

    // Consumables
    public const int ConsumablesTypeMaxNum = 9;
    public const int MaxConsumablesPerRoom = 3;
    public const int LevelUpdateDifficulty = 10;

    // Player/Item Stats
    public const int MinFoodRegen = 75;
    public const int MaxPercentFoodRegenFromHealth = 20;
    public const int MaxPercentAgilityIncrease = 10;
    public const int MaxPercentStrengthIncrease = 10;
    public const int MinElixirDurationSeconds = 30;
    public const int MaxElixirDurationSeconds = 60;
    public const int MinWeaponStrength = 30;
    public const int MaxWeaponStrength = 50;

    // Monster Stats
    public const int MaxMonstersPerRoom = 2;
    public const int PercentsUpdateDifficultyMonsters = 2;

    // Combat
    public const int InitialHitChance = 70;
    public const int StandardAgility = 50;
    public const double AgilityFactor = 0.3;
    public const double InitialDamage = 30;
    public const int StandardStrength = 50;
    public const double StrengthFactor = 0.3;
    public const int StrengthAddition = 65;
    public const int SleepChance = 15;
    public const double LootAgilityFactor = 0.2;
    public const double LootHpFactor = 0.5;
    public const double LootStrengthFactor = 0.5;

    // Movement
    public const int OgreStep = 2;
    public const int MaxTriesToMove = 16;

    public const int LowHostilityRadius = 2;
    public const int AverageHostilityRadius = 4;
    public const int HighHostilityRadius = 6;
}
