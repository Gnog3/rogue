using Rogue.Domain.Items;

namespace Rogue.Domain;

public sealed record Statistics
{
    public int Treasures { get; set; }
    public int Level { get; set; } = 1;
    public int Enemies { get; set; }
    public int Food { get; set; }
    public int Elixirs { get; set; }
    public int Scrolls { get; set; }
    public int Attacks { get; set; }
    public int Missed { get; set; }
    public int Moves { get; set; }

    public void ItemPickup(Item item)
    {
        if (item is Food)
        {
            Food++;
        }
        else if (item is Elixir)
        {
            Elixirs++;
        }
        else if (item is Scroll)
        {
            Scrolls++;
        }
    }
}
