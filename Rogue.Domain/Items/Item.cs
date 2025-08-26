using Rogue.Domain.Characters;

namespace Rogue.Domain.Items;

public abstract class Item(string name) : IHasPrinterName
{
    public abstract class Factory
    {
        public abstract Item Generate(Player player);
    }

    public static readonly Factory[] Factories =
    [
        new Elixir.Factory(),
        new Food.Factory(),
        new Scroll.Factory(),
        new Weapon.Factory(),
    ];

    public string Name { get; } = name;
    public abstract string PrinterName { get; }
}
