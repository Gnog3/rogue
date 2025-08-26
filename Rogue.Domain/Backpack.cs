using Rogue.Domain.Items;

namespace Rogue.Domain;

public sealed class Backpack
{
    public List<Food> Foods { get; } = [];
    public List<Elixir> Elixirs { get; } = [];
    public List<Scroll> Scrolls { get; } = [];
    public List<Weapon> Weapons { get; } = [];
    public int TreasureValue { get; set; }

    private static bool TryInsert<T>(List<T> items, Item item) where T : Item
    {
        if (item is not T typedItem || items.Count >= Constants.ConsumablesTypeMaxNum)
        {
            return false;
        }
        items.Add(typedItem);
        return true;
    }

    private static bool TryRemove<T>(List<T> items, Item item) where T : Item
    {
        if (item is not T typedItem)
        {
            return false;
        }
        return items.Remove(typedItem);
    }

    public bool TryInsert(Item item) =>
        TryInsert(Foods, item) ||
        TryInsert(Elixirs, item) ||
        TryInsert(Scrolls, item) ||
        TryInsert(Weapons, item);

    public void Remove(Item item) => _ =
        TryRemove(Foods, item) ||
        TryRemove(Elixirs, item) ||
        TryRemove(Scrolls, item) ||
        TryRemove(Weapons, item);
}
