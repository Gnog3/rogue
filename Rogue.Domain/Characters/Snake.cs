namespace Rogue.Domain.Characters;

public sealed class Snake(Vector position, double health, int agility, int strength) : Monster(position, health, agility, strength)
{
    public new sealed class Factory : Monster.Factory
    {
        public override double DefaultHealth => 100;
        public override int DefaultAgility => 100;
        public override int DefaultStrength => 30;
        public override Monster Create(Vector position, double health, int agility, int strength)
            => new Snake(position, health, agility, strength);
    }

    private Direction _lastDirection = Direction.Stop;
    private bool _nextAttackIsMissed;

    protected override int HostilityRadius => Constants.HighHostilityRadius;

    public override List<Direction> CalculatePattern(Level level)
    {
        // Try to move in a random diagonal direction, but not the same as the last direction
        for (int i = 0; i < Constants.MaxTriesToMove; i++)
        {
            Direction newDirection = DirectionHelper.RandomDiagonal();
            if (newDirection == _lastDirection)
            {
                continue;
            }

            Vector newPosition = this.Position + newDirection.Vector();
            if (level.IsInside(newPosition) && !level.IsOccupied(newPosition))
            {
                return [newDirection];
            }
        }

        // Otherwise, try to move the same direction as last time
        Vector newPosition2 = this.Position + _lastDirection.Vector();
        if (level.IsInside(newPosition2) && !level.IsOccupied(newPosition2))
        {
            return [_lastDirection];
        }

        return [];
    }

    public override bool CheckContact(Player player)
    {
        if (base.CheckContact(player))
        {
            return true;
        }

        // Check if the snake is adjacent to the player in diagonals
        Vector pos1 = this.Position;
        Vector pos2 = player.Position;

        return Math.Abs(pos1.X - pos2.X) == 1 && Math.Abs(pos1.Y - pos2.Y) == 1;
    }

    public override bool ReceiveAttack(Player player)
    {
        if (_nextAttackIsMissed)
        {
            _nextAttackIsMissed = false;
            return false;
        }

        return base.ReceiveAttack(player);
    }

    public override bool Attack(Player player)
    {
        bool success = base.Attack(player);
        if (success && Random.Shared.Next(100) < Constants.SleepChance)
        {
            _nextAttackIsMissed = true;
        }

        return success;
    }

    protected override void MoveByDirection(Direction direction)
    {
        _lastDirection = direction;
        base.MoveByDirection(direction);
    }
}
