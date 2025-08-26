using Rogue.Domain.Characters;

namespace Rogue.Domain.Items;

public sealed class Food(string name, int regen) : Consumable(name)
{
    public static readonly string[] Names =
    [
        "Ration of the Ironclad",
        "Crimson Berry Cluster",
        "Loaf of the Forgotten Baker",
        "Smoked Wyrm Jerky",
        "Golden Apple of Vitality",
        "Hardtack of the Endless March",
        "Spiced Venison Strips",
        "Honeyed Nectar Bread",
        "Dried Mushrooms of the Deep",
    ];

    public new class Factory : Item.Factory
    {
        public override Item Generate(Player player)
        {
            string name = Names[Random.Shared.Next(Names.Length)];
            int maxRegen = (int)(player.MaxHealth * Constants.MaxPercentFoodRegenFromHealth / 100);
            int regen =
                maxRegen <= Constants.MinFoodRegen
                    ? Constants.MinFoodRegen
                    : Random.Shared.Next(Constants.MinFoodRegen, maxRegen);
            return new Food(name, regen);
        }
    }

    public int Regen { get; } = regen;

    public override string PrinterName => "Food";

    public override void Use(Player player)
    {
        player.Health = Math.Min(player.MaxHealth, player.Health + Regen);
    }
}
