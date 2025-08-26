namespace Rogue.Domain.Characters;

public sealed class Zombie(Vector position, double health, int agility, int strength) : Monster(position, health, agility, strength)
{
    public new sealed class Factory : Monster.Factory
    {
        public override double DefaultHealth => 50;
        public override int DefaultAgility => 25;
        public override int DefaultStrength => 125;
        public override Monster Create(Vector position, double health, int agility, int strength)
            => new Zombie(position, health, agility, strength);
    }

    protected override int HostilityRadius => Constants.AverageHostilityRadius;

    // Move one tile in a random simple direction
    public override List<Direction> CalculatePattern(Level level)
    {
        List<Direction> path = [];

        for (int i = 0; i < Constants.MaxTriesToMove; i++)
        {
            Vector position = this.Position;
            Direction direction = DirectionHelper.RandomSimple();
            position += direction.Vector();
            if (level.IsInside(position) && !level.IsOccupied(position))
            {
                path.Add(direction);
                break;
            }
        }

        return path;
    }
}
