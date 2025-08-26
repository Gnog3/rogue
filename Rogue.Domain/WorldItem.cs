using Rogue.Domain.Items;

namespace Rogue.Domain;

public sealed class WorldItem(Item item, Vector position) : WorldObject(position)
{
    public Item Item { get; } = item;
}
