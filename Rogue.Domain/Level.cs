using Rogue.Domain.Characters;
using System.Diagnostics.CodeAnalysis;

namespace Rogue.Domain;

public sealed class Level(int levelNumber, Room[] rooms, Passage[] passages, Vector initialReveal)
{
    public int LevelNumber { get; } = levelNumber;
    public Room[] Rooms { get; } = rooms;
    public Passage[] Passages { get; } = passages;
    public List<WorldObject> Objects { get; } = [];

    public List<Room> RevealedRooms { get; } = [rooms.Single(r => r.IsInside(initialReveal))];
    public List<Passage> RevealedPassages { get; } = [];

    public bool IsInside(Vector position)
    {
        if (Rooms.Any(room => room.IsInside(position)))
        {
            return true;
        }

        return Passages.Any(passage => passage.Extent.Contains(position));
    }

    public bool IsRevealed(WorldObject worldObject)
    {
        if (RevealedRooms.Any(r => r.Extent.Contains(worldObject.Position)))
        {
            return true;
        }

        return RevealedPassages.Any(p => p.Extent.Contains(worldObject.Position));
    }

    public bool PlayerInTheSameObject(WorldObject worldObject, Player player)
    {
        if (RevealedRooms.Any(r => r.Extent.Contains(worldObject.Position) && r.Extent.Contains(player.Position)))
        {
            return true;
        }

        return RevealedPassages.Any(p => p.Extent.Contains(worldObject.Position) && p.Extent.Contains(player.Position));
    }

    public bool TryFindRoom(Vector position, [NotNullWhen(true)] out Room? result)
    {
        foreach (Room room in Rooms)
        {
            if (room.IsInside(position))
            {
                result = room;
                return true;
            }
        }
        result = null;
        return false;
    }

    public bool IsOccupied(Vector position) => Objects.Any(obj => obj.Position == position);

    public bool TryFindPath(Vector start, Vector end, [NotNullWhen(true)] out List<Direction>? result)
    {
        if (start == end)
        {
            result = [];
            return true;
        }

        var queue = new Queue<Vector>();
        queue.Enqueue(start);

        var visited = new bool[Constants.MapWidth, Constants.MapHeight];
        var parentMap = new Dictionary<Vector, (Vector parent, Direction dir)>();

        visited[start.X, start.Y] = true;

        while (queue.Count > 0)
        {
            Vector current = queue.Dequeue();

            if (current == end)
            {
                break;
            }

            foreach (var direction in DirectionHelper.SimpleDirections)
            {
                Vector newPosition = current + direction.Vector();
                if (this.IsInside(newPosition) && !this.IsOccupied(newPosition) && !visited[newPosition.X, newPosition.Y])
                {
                    visited[newPosition.X, newPosition.Y] = true;
                    parentMap[newPosition] = (current, direction);
                    queue.Enqueue(newPosition);
                }
            }
        }

        if (!parentMap.ContainsKey(end))
        {
            result = null;
            return false;
        }

        List<Direction> path = [];
        Vector currentPosition = end;

        while (currentPosition != start)
        {
            var (parent, dir) = parentMap[currentPosition];
            path.Add(dir);
            currentPosition = parent;
        }
        path.Reverse();
        result = path;
        return true;
    }
}
