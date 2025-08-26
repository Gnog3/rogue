using Rogue.Domain.Characters;

namespace Rogue.Domain.Items;

public abstract class Scroll(string name, int increase) : Consumable(name)
{
    public abstract class ScrollFactory
    {
        public abstract int MaxIncrease(Player player);
        public abstract Scroll Create(string name, int increase);
    }

    public static readonly ScrollFactory[] ScrollFactories =
    [
        new HealthScroll.ScrollFactory(),
        new AgilityScroll.ScrollFactory(),
        new StrengthScroll.ScrollFactory(),
    ];

    public new class Factory : Item.Factory
    {
        public override Item Generate(Player player)
        {
            string name = Names[Random.Shared.Next(Names.Length)];
            ScrollFactory factory = ScrollFactories[Random.Shared.Next(ScrollFactories.Length)];
            int maxIncrease = factory.MaxIncrease(player);
            int increase = Random.Shared.Next(maxIncrease);
            return factory.Create(name, increase);
        }
    }

    public static readonly string[] Names =
    [
        "Scroll of Shadowstep",
        "Parchment of Eternal Flame",
        "Manuscript of Forgotten Truths",
        "Scroll of Iron Will",
        "Vellum of the Void",
        "Scroll of Whispers",
        "Tome of the Lost King",
        "Scroll of Unseen Paths",
        "Parchment of Thunderous Roar",
    ];

    public int Increase { get; } = increase;

    public abstract string AffectedProperty { get; }

    public override string PrinterName => "Scroll";
}

public sealed class HealthScroll(string name, int increase) : Scroll(name, increase)
{
    public new class ScrollFactory : Scroll.ScrollFactory
    {
        public override int MaxIncrease(Player player) => (int)player.MaxHealth * Constants.MaxPercentFoodRegenFromHealth / 100;
        public override Scroll Create(string name, int increase) => new HealthScroll(name, increase);
    }

    public override string AffectedProperty => "health";

    public override void Use(Player player)
    {
        player.MaxHealth += Increase;
        player.Health += Increase;
    }
}

public sealed class AgilityScroll(string name, int increase) : Scroll(name, increase)
{
    public new class ScrollFactory : Scroll.ScrollFactory
    {
        public override int MaxIncrease(Player player) => (int)player.MaxHealth * Constants.MaxPercentAgilityIncrease / 100;
        public override Scroll Create(string name, int increase) => new AgilityScroll(name, increase);
    }

    public override string AffectedProperty => "agility";

    public override void Use(Player player)
    {
        player.Agility += Increase;
    }
}

public sealed class StrengthScroll(string name, int increase) : Scroll(name, increase)
{
    public new class ScrollFactory : Scroll.ScrollFactory
    {
        public override int MaxIncrease(Player player) => (int)player.MaxHealth * Constants.MaxPercentStrengthIncrease / 100;
        public override Scroll Create(string name, int increase) => new StrengthScroll(name, increase);
    }

    public override string AffectedProperty => "strength";

    public override void Use(Player player)
    {
        player.Strength += Increase;
    }
}
