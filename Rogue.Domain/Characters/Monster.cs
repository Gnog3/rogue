namespace Rogue.Domain.Characters;

public abstract class Monster(Vector position, double health, int agility, int strength) : Character(position, health, agility, strength), IHasPrinterName
{
    public abstract class Factory
    {
        public abstract double DefaultHealth { get; }
        public abstract int DefaultAgility { get; }
        public abstract int DefaultStrength { get; }
        public abstract Monster Create(Vector position, double health, int agility, int strength);
    }

    public static readonly Factory[] Factories =
    [
        new Ghost.Factory(),
        new Ogre.Factory(),
        new Snake.Factory(),
        new Vampire.Factory(),
        new Zombie.Factory(),
    ];

    protected abstract int HostilityRadius { get; }

    public string PrinterName => this.GetType().Name;

    public int CalculateLoot() =>
        (int)(
            Agility * Constants.LootAgilityFactor +
            Health * Constants.LootHpFactor +
            Strength * Constants.LootStrengthFactor +
            Random.Shared.NextInt64(20)
        );

    public abstract List<Direction> CalculatePattern(Level level);

    public virtual bool CheckContact(Player player)
    {
        Vector pos1 = this.Position;
        Vector pos2 = player.Position;

        // Check if the monster is adjacent to the player in simple directions
        return pos1.X == pos2.X && Math.Abs(pos1.Y - pos2.Y) == 1 ||
            pos1.Y == pos2.Y && Math.Abs(pos1.X - pos2.X) == 1;
    }

    public virtual bool ReceiveAttack(Player player)
    {
        if (Random.Shared.Next(100) >= player.HitChance(this))
        {
            return false;
        }

        double damage = Constants.InitialDamage;

        if (player.Weapon is null)
        {
            damage += (player.Strength - Constants.StandardStrength) * Constants.StrengthFactor;
        }
        else
        {
            damage = player.Weapon.Strength * (player.Strength + Constants.StrengthAddition) / 100;
        }

        Health -= damage;
        if (Health <= 0)
        {
            player.Backpack.TreasureValue += CalculateLoot();
        }

        return true;
    }

    public virtual bool Attack(Player player)
    {
        if (Random.Shared.Next(100) >= this.HitChance(player))
        {
            return false;
        }

        double damage = Constants.InitialDamage + (Strength - Constants.StandardStrength) * Constants.StrengthFactor;

        player.Health -= damage;
        return true;
    }

    // Is player within monster's hostility radius
    private bool IsPlayerNear(Player player)
    {
        int distance = Math.Abs(this.Position.X - player.Position.X) +
                   Math.Abs(this.Position.Y - player.Position.Y);

        return distance <= HostilityRadius;
    }

    protected virtual void MoveByDirection(Direction direction)
    {
        Position += direction.Vector();
    }

    // Move if it not ends in player position
    private void TryMoveByPath(IEnumerable<Direction> path, Player player)
    {
        Vector position = this.Position;
        foreach (Direction dir in path)
        {
            position += dir.Vector();
        }

        if (position == player.Position)
        {
            return;
        }

        foreach (Direction dir in path)
        {
            MoveByDirection(dir);
        }
    }

    public void Move(Player player, Level level)
    {
        // If player is near, move one tile towards him
        if (IsPlayerNear(player) && level.TryFindPath(Position, player.Position, out var result))
        {
            TryMoveByPath(result.Take(1), player);
            return;
        }

        // Otherwise, move according to the pattern
        var pattern = CalculatePattern(level);
        TryMoveByPath(pattern, player);
    }
}
