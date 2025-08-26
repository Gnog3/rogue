using Rogue.Domain.Items;

namespace Rogue.Presentation.States;

internal abstract class Select<T>(IState ret) : IState where T : Item
{
    protected abstract List<T> Items { get; }
    protected abstract string GetDisplayedItemName(T item);
    protected abstract void SelectItem(T? item);
    protected abstract string ItemTypeName { get; }

    public IState Update(char key)
    {
        if (key < '0' || key > '9')
        {
            return ret;
        }

        int selected = key - '0';
        if (selected == 0)
        {
            SelectItem(null);
            return ret;
        }

        int index = selected - 1;
        if (index < Items.Count)
        {
            SelectItem(Items[index]);
        }

        return ret;
    }

    public void Render()
    {
        Terminal.Instance.PutString(0, 0, Font.White, $"Choose {ItemTypeName}:");
        int y = 1;
        for (int i = 0; i < Items.Count; i++)
        {
            string s = $"{i + 1}. {GetDisplayedItemName(Items[i])}";
            Terminal.Instance.PutString(0, y, Font.White, s);
            y++;
        }

        if (Items.Count > 0)
        {
            string s = $"Press 1-{Items.Count} key to choose {ItemTypeName} or any key to continue";
            Terminal.Instance.PutString(0, y, Font.White, s);
        }
        else
        {
            Terminal.Instance.PutString(0, 1, Font.White, $"You don't have any {ItemTypeName}");
            Terminal.Instance.PutString(0, 2, Font.White, "Press any key to continue");
        }
    }
}

internal sealed class ElixirSelect(Rogue.Domain.Game game, IState ret) : Select<Elixir>(ret)
{
    protected override List<Elixir> Items => game.Player.Backpack.Elixirs;
    protected override string ItemTypeName => "elixir";
    protected override string GetDisplayedItemName(Elixir item) =>
        $"{item.Name} +{item.Increase} {item.AffectedProperty} for {item.Duration.Seconds} seconds";
    protected override void SelectItem(Elixir? item)
    {
        if (item is not null)
        {
            game.Player.DrinkElixir(item, game.GameTime);
        }
    }
}

internal sealed class FoodSelect(Rogue.Domain.Game game, IState ret) : Select<Food>(ret)
{
    protected override List<Food> Items => game.Player.Backpack.Foods;
    protected override string ItemTypeName => "food";
    protected override string GetDisplayedItemName(Food item) => $"{item.Name} +{item.Regen} health";
    protected override void SelectItem(Food? item)
    {
        if (item is not null)
        {
            game.Player.EatFood(item);
        }
    }
}

internal sealed class ScrollSelect(Rogue.Domain.Game game, IState ret) : Select<Scroll>(ret)
{
    protected override List<Scroll> Items => game.Player.Backpack.Scrolls;
    protected override string ItemTypeName => "scroll";
    protected override string GetDisplayedItemName(Scroll item) => $"{item.Name} +{item.Increase} {item.AffectedProperty}";
    protected override void SelectItem(Scroll? item)
    {
        if (item is not null)
        {
            game.Player.ReadScroll(item);
        }
    }
}

internal sealed class WeaponSelect(Rogue.Domain.Game game, IState ret) : Select<Weapon>(ret)
{
    protected override List<Weapon> Items => game.Player.Backpack.Weapons;
    protected override string ItemTypeName => "weapon";
    protected override string GetDisplayedItemName(Weapon item) => $"{item.Name} +{item.Strength} strength";
    protected override void SelectItem(Weapon? item)
    {
        game.Player.EquipWeapon(item, game.Level);
    }
}
