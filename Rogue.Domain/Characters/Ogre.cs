namespace Rogue.Domain.Characters;

public sealed class Ogre(Vector position, double health, int agility, int strength) : Monster(position, health, agility, strength)
{
    public new sealed class Factory : Monster.Factory
    {
        public override double DefaultHealth => 150;
        public override int DefaultAgility => 25;
        public override int DefaultStrength => 100;
        public override Monster Create(Vector position, double health, int agility, int strength)
            => new Ogre(position, health, agility, strength);
    }

    private bool _cooldown;

    protected override int HostilityRadius => Constants.AverageHostilityRadius;

    // Move two tiles in a random simple direction
    public override List<Direction> CalculatePattern(Level level)
    {
        for (int i = 0; i < Constants.MaxTriesToMove; i++)
        {
            Vector position = this.Position;
            Direction direction = DirectionHelper.RandomSimple();

            bool valid = true;
            for (int j = 0; j < Constants.OgreStep; j++)
            {
                position += direction.Vector();
                if (!level.IsInside(position) || level.IsOccupied(position))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                return [.. Enumerable.Repeat(direction, Constants.OgreStep)];
            }
        }

        return [];
    }

    public override int HitChance(Character target) => throw new NotImplementedException();

    public override bool Attack(Player player)
    {
        if (_cooldown)
        {
            _cooldown = false;
            return false; // Every other attack is missed
        }

        // No Constants.InitialDamage
        double damage = (Strength - Constants.StandardStrength) * Constants.StrengthFactor;
        player.Health -= damage;

        _cooldown = true;
        return true;
    }
}
