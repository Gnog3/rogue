namespace Rogue.Domain.Characters;

public sealed class Vampire(Vector position, double health, int agility, int strength) : Monster(position, health, agility, strength)
{
    public new sealed class Factory : Monster.Factory
    {
        public override double DefaultHealth => 50;
        public override int DefaultAgility => 75;
        public override int DefaultStrength => 125;
        public override Monster Create(Vector position, double health, int agility, int strength)
            => new Vampire(position, health, agility, strength);
    }

    protected override int HostilityRadius => Constants.HighHostilityRadius;

    private bool _playerAttacked;

    // Move one tile in a random direction (diagonal included)
    public override List<Direction> CalculatePattern(Level level)
    {
        List<Direction> path = [];

        for (int i = 0; i < Constants.MaxTriesToMove; i++)
        {
            Vector position = this.Position;
            Direction direction = DirectionHelper.RandomAll();
            position += direction.Vector();
            if (level.IsInside(position) && !level.IsOccupied(position))
            {
                path.Add(direction);
                break;
            }
        }

        return path;
    }

    public override bool ReceiveAttack(Player player)
    {
        if (!_playerAttacked)
        {
            // the first attack is always missed
            _playerAttacked = true;
            return false;
        }

        return base.ReceiveAttack(player);
    }

    public override bool Attack(Player player)
    {
        if (Random.Shared.Next(100) >= this.HitChance(player))
        {
            return false;
        }

        double damage = Constants.InitialDamage + (Strength - Constants.StandardStrength) * Constants.StrengthFactor;

        player.Health -= damage;
        player.MaxHealth -= damage;
        return true;
    }
}
