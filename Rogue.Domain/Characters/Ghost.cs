namespace Rogue.Domain.Characters;

public sealed class Ghost(Vector position, double health, int agility, int strength) : Monster(position, health, agility, strength)
{
    public new sealed class Factory : Monster.Factory
    {
        public override double DefaultHealth => 75;
        public override int DefaultAgility => 25;
        public override int DefaultStrength => 100;
        public override Monster Create(Vector position, double health, int agility, int strength)
            => new Ghost(position, health, agility, strength);
    }

    protected override int HostilityRadius => Constants.LowHostilityRadius;
    public bool IsHidden { get; private set; }

    // Random teleportation within the room
    public override List<Direction> CalculatePattern(Level level)
    {
        if (!level.TryFindRoom(this.Position, out Room? room))
        {
            throw new InvalidOperationException("Ghost must be inside a room to move.");
        }

        for (int i = 0; i < Constants.MaxTriesToMove; i++)
        {
            Vector newPosition = room.InnerExtent.RandomVector();
            if (level.IsInside(newPosition) && !level.IsOccupied(newPosition))
            {
                if (level.TryFindPath(this.Position, newPosition, out List<Direction>? path))
                {
                    return path;
                }
            }
        }

        return [];
    }

    public override bool ReceiveAttack(Player player)
    {
        IsHidden = false;
        return base.ReceiveAttack(player);
    }

    public override bool Attack(Player player)
    {
        IsHidden = false;
        return base.Attack(player);
    }

    protected override void MoveByDirection(Direction direction)
    {
        IsHidden = !IsHidden;
        base.MoveByDirection(direction);
    }
}
