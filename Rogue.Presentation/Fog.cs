using Rogue.Domain;
using System.Diagnostics;

namespace Rogue.Presentation;

internal static class Fog
{
    public static void RenderFog(Game game)
    {
        foreach (var room in game.Level.Rooms)
        {
            // Room is not revealed, just skip
            if (!game.Level.RevealedRooms.Contains(room))
            {
                continue;
            }

            // Player is outside the room completely, full fog
            if (!room.Extent.Contains(game.Player.Position))
            {
                FullFog(room);
                continue;
            }

            // Player is inside the room, no fog
            if (room.InnerExtent.Contains(game.Player.Position))
            {
                continue;
            }

            // Player is in the walls, partial fog
            Direction direction;
            if (room.Extent.Position.X == game.Player.Position.X)
            {
                direction = Direction.Right;
            }
            else if (room.Extent.Position.Y == game.Player.Position.Y)
            {
                direction = Direction.Back;
            }
            else if (room.Extent.Position.X + room.Extent.Size.X - 1 == game.Player.Position.X)
            {
                direction = Direction.Left;
            }
            else if (room.Extent.Position.Y + room.Extent.Size.Y - 1 == game.Player.Position.Y)
            {
                direction = Direction.Forward;
            }
            else
            {
                throw new UnreachableException();
            }

            PartialFog(room, direction, game.Player.Position);
        }
    }

    private static void FullFog(Room room)
    {
        for (int y = 0; y < room.InnerExtent.Size.Y; y++)
        {
            for (int x = 0; x < room.InnerExtent.Size.X; x++)
            {
                int screenX = room.InnerExtent.Position.X + x;
                int screenY = room.InnerExtent.Position.Y + y;
                Terminal.Instance.Put(screenX, screenY, new MapChar('.', Font.White));
            }
        }
    }

    private static void PartialFog(Room room, Direction direction, Vector position)
    {
        Vector from = position + direction.Vector();
        Vector to = position + direction.Vector();

        var unfog = new bool[room.InnerExtent.Size.X, room.InnerExtent.Size.Y];

        Vector left = direction switch
        {
            Direction.Forward => new Vector(-1, 0),
            Direction.Back => new Vector(1, 0),
            Direction.Left => new Vector(0, 1),
            Direction.Right => new Vector(0, -1),
            _ => throw new UnreachableException(),
        };
        Vector right = left * -1;

        while (!Edge(room, from))
        {
            for (Vector v = from; v != to + right; v += right)
            {
                Vector fromRoom = v - room.InnerExtent.Position;
                unfog[fromRoom.X, fromRoom.Y] = true;
            }

            if (!Edge(room, from + left))
            {
                from += left;
            }

            if (!Edge(room, to + right))
            {
                to += right;
            }

            from += direction.Vector();
            to += direction.Vector();
        }

        for (int y = 0; y < unfog.GetLength(1); y++)
        {
            for (int x = 0; x < unfog.GetLength(0); x++)
            {
                if (unfog[x, y])
                {
                    continue;
                }

                int screenX = x + room.InnerExtent.Position.X;
                int screenY = y + room.InnerExtent.Position.Y;
                Terminal.Instance.Put(screenX, screenY, new MapChar('.', Font.White));
            }
        }
    }

    private static bool Edge(Room room, Vector position)
        => room.Extent.Contains(position) && !room.InnerExtent.Contains(position);
}
