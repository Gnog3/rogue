using Rogue.Domain.Characters;

namespace Rogue.Domain.Items;

public sealed class Weapon(int strength, string name) : Item(name)
{
    public static readonly string[] Names =
    [
        "Blade of the Forgotten Dawn",
        "Obsidian Reaver",
        "Fang of the Shadow Wolf",
        "Ironclad Cleaver",
        "Crimson Talon",
        "Thunderstrike Maul",
        "Serpent's Kiss Dagger",
        "Voidrend Sword",
        "Ebonheart Spear",
    ];

    public new class Factory : Item.Factory
    {
        public override Item Generate(Player player)
        {
            string name = Names[Random.Shared.Next(Names.Length)];
            int strength = Random.Shared.Next(Constants.MinWeaponStrength, Constants.MaxWeaponStrength);
            return new Weapon(strength, name);
        }
    }

    public int Strength { get; } = strength;

    public override string PrinterName => "Weapon";
}
