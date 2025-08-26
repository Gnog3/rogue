using Rogue.Domain.Items;

namespace Rogue.Domain;

public abstract record PlayerAction
{
    public sealed record Move(Direction Direction) : PlayerAction;
    public sealed record ReadScroll(Scroll Scroll) : PlayerAction;
    public sealed record DrinkElixir(Elixir Elixir) : PlayerAction;
    public sealed record EatFood(Food Food) : PlayerAction;
    public sealed record EquipWeapon(Weapon? Weapon) : PlayerAction;
}
