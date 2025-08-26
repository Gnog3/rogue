using Rogue.Domain;
using Rogue.Domain.Characters;
using Rogue.Domain.Items;
using System.Diagnostics;

namespace Rogue.Presentation.States;

internal sealed class Game(Rogue.Domain.Game game) : IState
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly INotifier _notifier = new Notifier();

    private static readonly IWorldObjectChar[] WorldObjectChars =
    [
        new AssignableFromWorldObjectChar<Player>(new MapChar('@', Font.White)),
        new AssignableFromWorldObjectChar<Exit>(new MapChar('E', Font.Cyan)),

        new GhostWorldObjectChar(),
        new AssignableFromWorldObjectChar<Ogre>(new MapChar('O', Font.Yellow)),
        new AssignableFromWorldObjectChar<Snake>(new MapChar('s', Font.White)),
        new AssignableFromWorldObjectChar<Vampire>(new MapChar('v', Font.Red)),
        new AssignableFromWorldObjectChar<Zombie>(new MapChar('z', Font.Green)),

        new WorldItemWorldObjectChar<Elixir>(new MapChar('e', Font.Green)),
        new WorldItemWorldObjectChar<Food>(new MapChar('f', Font.Red)),
        new WorldItemWorldObjectChar<Scroll>(new MapChar('S', Font.Green)),
        new WorldItemWorldObjectChar<Weapon>(new MapChar('w', Font.Red)),
    ];

    public IState Update(char key)
    {
        game.AdvanceTime(_stopwatch.Elapsed);
        _stopwatch.Restart();

        if (Controls.Up.Contains(key))
        {
            game.ProcessPlayerAction(new PlayerAction.Move(Direction.Forward), _notifier);
        }
        else if (Controls.Down.Contains(key))
        {
            game.ProcessPlayerAction(new PlayerAction.Move(Direction.Back), _notifier);
        }
        else if (Controls.Left.Contains(key))
        {
            game.ProcessPlayerAction(new PlayerAction.Move(Direction.Left), _notifier);
        }
        else if (Controls.Right.Contains(key))
        {
            game.ProcessPlayerAction(new PlayerAction.Move(Direction.Right), _notifier);
        }
        else if (Controls.ScrollSelect.Contains(key))
        {
            return new ScrollSelect(game, this);
        }
        else if (Controls.WeaponSelect.Contains(key))
        {
            return new WeaponSelect(game, this);
        }
        else if (Controls.FoodSelect.Contains(key))
        {
            return new FoodSelect(game, this);
        }
        else if (Controls.ElixirSelect.Contains(key))
        {
            return new ElixirSelect(game, this);
        }
        else if (Controls.Quit.Contains(key))
        {
            try
            {
                Data.Data.SaveData(game);
            }
            catch (Exception ex)
            {
                Terminal.Instance.PutMessage($"Error: {ex.Message}");
            }

            return new MainMenu();
        }

        if (game.IsWon)
        {
            SaveStatistics();
            return new Fullscreen(Fullscreen.Win, () => new MainMenu());
        }

        if (game.IsOver)
        {
            SaveStatistics();

            return new Fullscreen(Fullscreen.GameOver, () => new MainMenu());
        }

        if (game.OnExit)
        {
            game.Level = Generation.GenerateLevel(game.Level.LevelNumber + 1, game.Player);
        }

        return this;
    }

    public void Render()
    {
        RenderRooms();
        RenderPassages();
        RenderWorldObjects();
        RenderStatusBar();

        Fog.RenderFog(game);
    }

    private void RenderRooms()
    {
        foreach (var room in game.Level.Rooms.Where(r => game.Level.RevealedRooms.Contains(r)))
        {
            for (int x = 0; x < room.Extent.Size.X; x++)
            {
                Terminal.Instance.Put(room.Extent.Position.X + x, room.Extent.Position.Y, new MapChar('-', Font.White));
                Terminal.Instance.Put(room.Extent.Position.X + x, room.Extent.Position.Y + room.Extent.Size.Y - 1, new MapChar('-', Font.White));
            }

            for (int y = 1; y < room.Extent.Size.Y - 1; y++)
            {
                Terminal.Instance.Put(room.Extent.Position.X, room.Extent.Position.Y + y, new MapChar('|', Font.White));
                Terminal.Instance.Put(room.Extent.Position.X + room.Extent.Size.X - 1, room.Extent.Position.Y + y, new MapChar('|', Font.White));
            }
        }
    }

    private void RenderPassageChar(Vector position, bool isRevealed)
    {
        bool onBorder = false;
        bool roomRevealed = false;
        foreach (var room in game.Level.Rooms)
        {
            if (room.Extent.Contains(position))
            {
                onBorder = true;
                roomRevealed = game.Level.RevealedRooms.Contains(room);
                break;
            }
        }

        if (onBorder && (isRevealed || roomRevealed))
        {
            Terminal.Instance.Put(position.X, position.Y, new MapChar('+', Font.White));
        }
        else if (isRevealed)
        {
            Terminal.Instance.Put(position.X, position.Y, new MapChar('#', Font.White));
        }
    }

    private void RenderPassages()
    {
        foreach (var passage in game.Level.Passages)
        {
            bool isRevealedPassage = game.Level.RevealedPassages.Contains(passage);
            if (passage.Extent.Size.Y == 1)
            {
                for (int x = 0; x < passage.Extent.Size.X; x++)
                {
                    RenderPassageChar(new Vector(passage.Extent.Position.X + x, passage.Extent.Position.Y), isRevealedPassage);
                }
            }
            else
            {
                for (int y = 0; y < passage.Extent.Size.Y; y++)
                {
                    RenderPassageChar(new Vector(passage.Extent.Position.X, passage.Extent.Position.Y + y), isRevealedPassage);
                }
            }
        }
    }

    private void RenderWorldObjects()
    {
        RenderWorldObjects([game.Player]);
        RenderWorldObjects(game.Level.Objects);
    }

    private void RenderWorldObjects(IEnumerable<WorldObject> objects)
    {
        foreach (var obj in objects)
        {
            if (!game.Level.PlayerInTheSameObject(obj, game.Player))
            {
                continue;
            }
            var provider = WorldObjectChars.SingleOrDefault(p => p.IsCompatible(obj))
                ?? throw new Exception($"Can't render world object: {obj.GetType()}");
            Terminal.Instance.Put(obj.Position.X, obj.Position.Y, provider.Char(obj));
        }
    }

    private void RenderStatusBar()
    {
        string level = $"Level: {game.Level.LevelNumber}";
        string gold = $"Gold: {game.Player.Backpack.TreasureValue}";
        string health = $"Health:{game.Player.Health:F2}/{game.Player.MaxHealth:F2}";
        string agility = $"Agility:{game.Player.Agility}";
        string strength = $"Strength: {game.Player.Strength}(+{game.Player.Weapon?.Strength ?? 0})";
        string statusBar = string.Join("  ", level, gold, health, agility, strength);
        Terminal.Instance.StatusBar = statusBar;
    }

    private void SaveStatistics()
    {
        try
        {
            List<Statistics> statistics = Data.Data.LoadStatistics();
            statistics.Add(game.Statistics);
            Data.Data.SaveStatistics(statistics);
        }
        catch (Exception ex)
        {
            Terminal.Instance.PutMessage($"Error: {ex.Message}");
        }
    }
}

internal class Notifier : INotifier
{
    public void FullBackpack(Item item)
    {
        Terminal.Instance.PutMessage($"Can't put {item.PrinterName.ToLower()}, backpack is full.");
    }

    public void MonsterAttacked(Monster monster)
    {
        Terminal.Instance.PutMessage($"{monster.PrinterName} attacked you.");
    }

    public void MonsterMissed(Monster monster)
    {
        Terminal.Instance.PutMessage($"{monster.PrinterName} missed.");
    }

    public void PlayerAttacked(Monster monster)
    {
        Terminal.Instance.PutMessage($"You attacked {monster.PrinterName.ToLower()}.");
    }

    public void PlayerMissed(Monster monster)
    {
        Terminal.Instance.PutMessage("You missed.");
    }

    public void PlayerPickedUp(Item item)
    {
        Terminal.Instance.PutMessage($"You picked up {item.PrinterName.ToLower()}.");
    }
}

internal interface IWorldObjectChar
{
    MapChar Char(WorldObject worldObject);
    bool IsCompatible(WorldObject worldObject);
}

internal sealed class AssignableFromWorldObjectChar<T>(MapChar mapChar) : IWorldObjectChar where T : WorldObject
{
    public MapChar Char(WorldObject worldObject) => mapChar;
    public bool IsCompatible(WorldObject worldObject) => worldObject is T;
}

internal sealed class WorldItemWorldObjectChar<T>(MapChar mapChar) : IWorldObjectChar where T : Item
{
    public MapChar Char(WorldObject worldObject) => mapChar;

    public bool IsCompatible(WorldObject worldObject)
    {
        if (worldObject is not WorldItem worldItem)
        {
            return false;
        }

        return worldItem.Item is T;
    }
}

internal sealed class GhostWorldObjectChar : IWorldObjectChar
{
    public MapChar Char(WorldObject worldObject)
    {
        if (worldObject is not Ghost ghost)
        {
            throw new ArgumentException("World object is not a Ghost.");
        }

        return new MapChar(ghost.IsHidden ? ' ' : 'g', Font.White);
    }

    public bool IsCompatible(WorldObject worldObject) => worldObject is Ghost;
}
