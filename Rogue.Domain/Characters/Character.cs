namespace Rogue.Domain.Characters;

public abstract class Character(Vector position, double health, int agility, int strength) : WorldObject(position)
{
    public double Health { get; set; } = health;
    public int Agility { get; set; } = agility;
    public int Strength { get; set; } = strength;

    public virtual int HitChance(Character target) =>
        Constants.InitialHitChance + (int)((Agility - target.Agility - Constants.StandardAgility) * Constants.AgilityFactor);
}
