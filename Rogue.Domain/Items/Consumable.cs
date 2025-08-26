using Rogue.Domain.Characters;

namespace Rogue.Domain.Items;

public abstract class Consumable(string name) : Item(name)
{
    public abstract void Use(Player player);
}
