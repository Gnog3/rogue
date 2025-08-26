using Rogue.Domain.Characters;

namespace Rogue.Domain.Items;

public abstract class Elixir(string name, int increase, TimeSpan duration) : Consumable(name)
{
    public abstract class ElixirFactory
    {
        public abstract int MaxIncrease(Player player);
        public abstract Elixir Create(string name, int increase, TimeSpan duration);
    }

    public static readonly ElixirFactory[] ElixirFactories =
    [
        new HealthElixir.ElixirFactory(),
        new AgilityElixir.ElixirFactory(),
        new StrengthElixir.ElixirFactory(),
    ];

    public new class Factory : Item.Factory
    {
        public override Item Generate(Player player)
        {
            string name = Names[Random.Shared.Next(Names.Length)];
            ElixirFactory factory = ElixirFactories[Random.Shared.Next(ElixirFactories.Length)];
            int maxIncrease = factory.MaxIncrease(player);
            int increase = Random.Shared.Next(maxIncrease);
            int duration = Random.Shared.Next(Constants.MinElixirDurationSeconds, Constants.MaxElixirDurationSeconds + 1);
            return factory.Create(name, increase, TimeSpan.FromSeconds(duration));
        }
    }

    public static readonly string[] Names =
    [
        "Elixir of the Jade Serpent",
        "Potion of the Phantom's Breath",
        "Vial of Crimson Vitality",
        "Draught of the Frozen Star",
        "Elixir of the Shattered Mind",
        "Potion of the Wandering Soul",
        "Vial of Ember Essence",
        "Elixir of the Obsidian Veil",
        "Potion of the Howling Wind",
    ];

    public int Increase { get; } = increase;
    public TimeSpan Duration { get; } = duration;

    public override string PrinterName => "Elixir";

    public abstract string AffectedProperty { get; }
    public abstract void Unuse(Player player);
}

public sealed class HealthElixir(string name, int increase, TimeSpan duration) : Elixir(name, increase, duration)
{
    public new class ElixirFactory : Elixir.ElixirFactory
    {
        public override int MaxIncrease(Player player) => (int)player.MaxHealth * Constants.MaxPercentFoodRegenFromHealth / 100;
        public override Elixir Create(string name, int increase, TimeSpan duration) => new HealthElixir(name, increase, duration);
    }

    public override string AffectedProperty => "health";

    public override void Use(Player player)
    {
        player.MaxHealth += Increase;
        player.Health += Increase;
    }

    public override void Unuse(Player player)
    {
        player.MaxHealth = Math.Max(1, player.MaxHealth - Increase);
        player.Health = Math.Max(1, player.Health - Increase);
    }
}

public sealed class AgilityElixir(string name, int increase, TimeSpan duration) : Elixir(name, increase, duration)
{
    public new class ElixirFactory : Elixir.ElixirFactory
    {
        public override int MaxIncrease(Player player) => player.Agility * Constants.MaxPercentAgilityIncrease / 100;
        public override Elixir Create(string name, int increase, TimeSpan duration) => new AgilityElixir(name, increase, duration);
    }

    public override string AffectedProperty => "agility";

    public override void Use(Player player)
    {
        player.Agility += Increase;
    }

    public override void Unuse(Player player)
    {
        player.Agility -= Increase;
    }
}

public sealed class StrengthElixir(string name, int increase, TimeSpan duration) : Elixir(name, increase, duration)
{
    public new class ElixirFactory : Elixir.ElixirFactory
    {
        public override int MaxIncrease(Player player) => player.Strength * Constants.MaxPercentStrengthIncrease / 100;
        public override Elixir Create(string name, int increase, TimeSpan duration) => new StrengthElixir(name, increase, duration);
    }

    public override string AffectedProperty => "strength";

    public override void Use(Player player)
    {
        player.Strength += Increase;
    }

    public override void Unuse(Player player)
    {
        player.Strength -= Increase;
    }
}
