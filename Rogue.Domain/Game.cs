using Rogue.Domain.Characters;

namespace Rogue.Domain;

public sealed class Game(Level level, Player player)
{
    public Level Level { get; set; } = level;
    public Player Player { get; set; } = player;
    public Statistics Statistics { get; set; } = new();
    public TimeSpan GameTime { get; set; } = TimeSpan.Zero;
    public bool OnExit => Level.Objects.OfType<Exit>().Single().Position == Player.Position;
    public bool IsOver => Player.Health <= 0;
    public bool IsWon => OnExit && Level.LevelNumber == Constants.MaxLevels;

    private void ProcessPlayerMove(Direction direction, INotifier notifier)
    {
        Vector newPosition = Player.Position + direction.Vector();

        bool blockMove = false;

        // Checking if the player attacking a monster
        Monster? monsterBeingAttacked = Level.Objects
            .OfType<Monster>()
            .SingleOrDefault(m => m.CheckContact(Player) && m.Position == newPosition);
        if (monsterBeingAttacked is not null)
        {
            Statistics.Attacks++;
            bool success = monsterBeingAttacked.ReceiveAttack(Player);
            if (success)
            {
                notifier.PlayerAttacked(monsterBeingAttacked);
            }
            else
            {
                notifier.PlayerMissed(monsterBeingAttacked);
                Statistics.Missed++;
            }

            if (monsterBeingAttacked.Health <= 0)
            {
                Statistics.Enemies++;
                Level.Objects.Remove(monsterBeingAttacked);
            }

            blockMove = true;
        }

        // Check for expired buffs
        Player.Buffs.RemoveAll(buff =>
        {
            if (buff.IsExpired(GameTime))
            {
                // side effect in linq
                buff.Elixir.Unuse(Player);
                return true;
            }
            return false;
        });

        // Try to pick up an item
        WorldItem? standingOn = Level.Objects
            .OfType<WorldItem>()
            .FirstOrDefault(item => item.Position == newPosition);
        if (standingOn is not null)
        {
            if (Player.Backpack.TryInsert(standingOn.Item))
            {
                Level.Objects.Remove(standingOn);
                notifier.PlayerPickedUp(standingOn.Item);
                Statistics.ItemPickup(standingOn.Item);
            }
            else
            {
                notifier.FullBackpack(standingOn.Item);
                blockMove = true;
            }
        }

        // Set the new position
        if (!blockMove && Level.IsInside(newPosition))
        {
            Player.Position = newPosition;
        }

        // Handle revelation
        Room? room = Level.Rooms.SingleOrDefault(r => r.Extent.Contains(Player.Position));
        if (room is not null && !Level.RevealedRooms.Contains(room))
        {
            Level.RevealedRooms.Add(room);
        }

        foreach (var passage in Level.Passages.Where(p => p.Extent.Contains(Player.Position)))
        {
            if (!Level.RevealedPassages.Contains(passage))
            {
                Level.RevealedPassages.Add(passage);
            }
        }

        Statistics.Moves++;
        Statistics.Treasures = Player.Backpack.TreasureValue;
        Statistics.Level = Level.LevelNumber;
    }

    private void ProcessMonsterMove(INotifier notifier)
    {

        foreach (var monster in Level.Objects.OfType<Monster>())
        {
            if (!monster.CheckContact(Player))
            {
                // Monsters move if they're not in a fight
                monster.Move(Player, Level);
            }
            else
            {
                // Monsters attack
                bool success = monster.Attack(Player);
                if (success)
                {
                    notifier.MonsterAttacked(monster);
                }
                else
                {
                    notifier.MonsterMissed(monster);
                }
            }
        }
    }

    public void ProcessPlayerAction(PlayerAction action, INotifier notifier)
    {
        switch (action)
        {
            case PlayerAction.Move move:
                ProcessPlayerMove(move.Direction, notifier);
                ProcessMonsterMove(notifier);
                break;
            case PlayerAction.ReadScroll readScroll:
                Player.ReadScroll(readScroll.Scroll);
                break;
            case PlayerAction.DrinkElixir drinkElixir:
                Player.DrinkElixir(drinkElixir.Elixir, GameTime);
                break;
            case PlayerAction.EatFood eatFood:
                Player.EatFood(eatFood.Food);
                break;
            case PlayerAction.EquipWeapon equipWeapon:
                Player.EquipWeapon(equipWeapon.Weapon, Level);
                break;
            default:
                throw new Exception($"Unhandled player action: {action}");
        }
    }

    public void AdvanceTime(TimeSpan time)
    {
        GameTime += time;
    }
}
