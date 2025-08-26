using Rogue.Domain.Items;

namespace Rogue.Domain.Characters;

public sealed class Player(Vector position) : Character(position, health: 500, agility: 70, strength: 70)
{
    public double MaxHealth { get; set; } = 500;
    public List<Buff> Buffs { get; } = [];
    public Weapon? Weapon { get; set; }
    public Backpack Backpack { get; } = new();

    public void ReadScroll(Scroll scroll)
    {
        scroll.Use(this);
        Backpack.Remove(scroll);
    }

    public void DrinkElixir(Elixir elixir, TimeSpan gameTime)
    {
        elixir.Use(this);
        Buffs.Add(new Buff(elixir, gameTime));
        Backpack.Remove(elixir);
    }

    public void EatFood(Food food)
    {
        food.Use(this);
        Backpack.Remove(food);
    }

    public void EquipWeapon(Weapon? weapon, Level level)
    {
        if (weapon is null)
        {
            Weapon = null;
            return;
        }

        if (weapon == Weapon)
        {
            return;
        }

        if (Weapon is not null)
        {
            WorldItem item = new(Weapon, this.Position);
            bool validPosition = false;
            // Try to find a position to drop the currently equipped weapon
            foreach (var direction in DirectionHelper.SimpleDirections)
            {
                Vector newPosition = item.Position + direction.Vector();

                if (level.IsInside(newPosition) && !level.IsOccupied(newPosition))
                {
                    item.Position = newPosition;
                    level.Objects.Add(item);
                    Backpack.Weapons.Remove(Weapon);
                    validPosition = true;
                    break;
                }
            }

            if (!validPosition)
            {
                return; // Can't drop the equipped weapon   
            }
        }

        Weapon = weapon;
    }
}
