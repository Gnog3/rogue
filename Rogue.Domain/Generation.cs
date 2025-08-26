using MoreStructures.DisjointSets;
using Rogue.Domain.Characters;
using Rogue.Domain.Items;

namespace Rogue.Domain;

public static class Generation
{
    public static Level GenerateLevel(int levelNumber, Player player)
    {
        Room[] rooms = GenerateRooms();
        Passage[] passages = GeneratePassages(rooms);

        Room playerRoom = SetPlayerPosition(player, rooms);

        Level level = new(levelNumber, rooms, passages, player.Position);

        GenerateMonsters(level, playerRoom);
        GenerateItems(level, player, playerRoom);
        GenerateExit(level, playerRoom);

        return level;
    }

    private static Room[] GenerateRooms()
    {
        Room[] rooms = new Room[Constants.RoomsNum];

        for (int i = 0; i < Constants.RoomsNum; i++)
        {
            int width = Random.Shared.Next(Constants.MinRoomWidth, Constants.MaxRoomWidth + 1);
            int height = Random.Shared.Next(Constants.MinRoomHeight, Constants.MaxRoomHeight + 1);

            int leftRangeCoord = (i % Constants.RoomsInWidth) * Constants.RegionWidth + 1;
            int rightRangeCoord = (i % Constants.RoomsInWidth + 1) * Constants.RegionWidth - width;
            int x = Random.Shared.Next(leftRangeCoord, rightRangeCoord);

            int upRangeCoord = (i / Constants.RoomsInWidth) * Constants.RegionHeight + 1;
            int bottomRangeCoord = (i / Constants.RoomsInWidth + 1) * Constants.RegionHeight - height;
            int y = Random.Shared.Next(upRangeCoord, bottomRangeCoord);

            rooms[i] = new Room(new Extent(new Vector(x, y), new Vector(width, height)));
        }

        return rooms;
    }

    private static Passage[] GenerateHorizontalPassage(Room room1, Room room2)
    {
        Vector firstCoords = room1.Extent.Position;
        Vector firstSize = room1.Extent.Size;
        Vector secondCoords = room2.Extent.Position;
        Vector secondSize = room2.Extent.Size;

        int firstX = firstCoords.X + firstSize.X - 1;
        int upRangeCoord = firstCoords.Y + 1;
        int bottomRangeCoord = firstCoords.Y + firstSize.Y - 1;
        int firstY = Random.Shared.Next(upRangeCoord, bottomRangeCoord);

        int secondX = secondCoords.X;
        upRangeCoord = secondCoords.Y + 1;
        bottomRangeCoord = secondCoords.Y + secondSize.Y - 1;
        int secondY = Random.Shared.Next(upRangeCoord, bottomRangeCoord);

        if (firstY == secondY)
        {
            return
            [
                new Passage(new Extent(
                    new Vector(firstX, firstY),
                    new Vector(Math.Abs(secondX - firstX) + 1, 1)
                ))
            ];
        }
        else
        {
            int vertical = Random.Shared.Next(Math.Min(firstX, secondX) + 1, Math.Max(firstX, secondX));

            return
            [
                new Passage(new Extent(
                    new Vector(firstX, firstY),
                    new Vector(Math.Abs(vertical - firstX) + 1, 1)
                )),
                new Passage(new Extent(
                    new Vector(vertical, Math.Min(firstY, secondY)),
                    new Vector(1, Math.Abs(secondY - firstY) + 1)
                )),
                new Passage(new Extent(
                    new Vector(vertical, secondY),
                    new Vector(Math.Abs(secondX - vertical) + 1, 1)
                ))
            ];
        }
    }

    private static Passage[] GenerateVerticalPassage(Room room1, Room room2)
    {
        Vector firstCoords = room1.Extent.Position;
        Vector firstSize = room1.Extent.Size;
        Vector secondCoords = room2.Extent.Position;
        Vector secondSize = room2.Extent.Size;

        int firstY = firstCoords.Y + firstSize.Y - 1;
        int upRangeCoord = firstCoords.X + 1;
        int bottomRangeCoord = firstCoords.X + firstSize.X - 2;
        int firstX = Random.Shared.Next(upRangeCoord, bottomRangeCoord + 1);

        int secondY = secondCoords.Y;
        upRangeCoord = secondCoords.X + 1;
        bottomRangeCoord = secondCoords.X + secondSize.X - 2;
        int secondX = Random.Shared.Next(upRangeCoord, bottomRangeCoord + 1);

        if (firstX == secondX)
        {
            return
            [
                new Passage(new Extent(
                    new Vector(firstX, firstY),
                    new Vector(1, Math.Abs(secondY - firstY) + 1)
                ))
            ];
        }
        int horizont = Random.Shared.Next(Math.Min(firstY, secondY) + 1, Math.Max(firstY, secondY));

        return
        [
            new Passage(new Extent(
                new Vector(firstX, firstY),
                new Vector(1, Math.Abs(horizont - firstY) + 1)
            )),
            new Passage(new Extent(
                new Vector(Math.Min(firstX, secondX), horizont),
                new Vector(Math.Abs(secondX - firstX) + 1, 1)
            )),
            new Passage(new Extent(
                new Vector(secondX, horizont),
                new Vector(1, Math.Abs(secondY - horizont) + 1)
            ))
        ];
    }

    private static (int, int)[] GenerateEdges()
    {
        List<(int, int)> edges = [];
        for (int i = 0; i < Constants.RoomsNum; i++)
        {
            int y = i / Constants.RoomsInWidth;
            int x = i % Constants.RoomsInWidth;

            if (x != Constants.RoomsInWidth - 1)
            {
                edges.Add((i, i + 1));
            }

            if (y != Constants.RoomsInHeight - 1)
            {
                edges.Add((i, i + Constants.RoomsInWidth));
            }
        }

        return [.. edges];
    }

    private static Passage[] GeneratePassages(Room[] rooms)
    {
        (int, int)[] edges = GenerateEdges();
        Random.Shared.Shuffle(edges);

        QuickFindDisjointSet set = new(9);

        List<Passage> passages = [];

        foreach (var edge in edges)
        {
            if (set.SetsCount == 1)
            {
                break;
            }

            if (set.AreConnected(edge.Item1, edge.Item2))
            {
                continue;
            }

            switch (edge.Item2 - edge.Item1)
            {
                case 1:
                    passages.AddRange(GenerateHorizontalPassage(rooms[edge.Item1], rooms[edge.Item2]));
                    break;
                case 3:
                    passages.AddRange(GenerateVerticalPassage(rooms[edge.Item1], rooms[edge.Item2]));
                    break;
                default:
                    throw new Exception("Invalid edge");
            }

            set.Union(edge.Item1, edge.Item2);
        }

        return [.. passages];
    }

    private static Room SetPlayerPosition(Player player, Room[] rooms)
    {
        Room room = rooms[Random.Shared.Next(rooms.Length)];
        player.Position = room.InnerExtent.RandomVector();
        return room;
    }

    private static void GenerateMonsters(Level level, Room playerRoom)
    {
        int maxMonsters = Constants.MaxMonstersPerRoom + level.LevelNumber / Constants.LevelUpdateDifficulty;
        foreach (var room in level.Rooms)
        {
            if (room == playerRoom)
            {
                continue;
            }

            int numberOfMonsters = Random.Shared.Next(maxMonsters);
            for (int i = 0; i < numberOfMonsters; i++)
            {
                Monster monster = GenerateMonster(level, room);
                level.Objects.Add(monster);
            }
        }
    }

    private static Vector GenerateFreeVector(Level level, Room room)
    {
        while (true)
        {
            Vector position = room.InnerExtent.RandomVector();
            if (level.IsOccupied(position))
            {
                continue;
            }

            return position;
        }
    }

    private static Monster GenerateMonster(Level level, Room room)
    {
        while (true)
        {
            Vector position = GenerateFreeVector(level, room);

            int monsterTypes = Monster.Factories.Length;
            Monster.Factory factory = Monster.Factories[Random.Shared.Next(monsterTypes)];

            int percentsUpdate = 100 + Constants.PercentsUpdateDifficultyMonsters * level.LevelNumber;
            double health = factory.DefaultHealth * percentsUpdate / 100;
            int agility = factory.DefaultAgility * percentsUpdate / 100;
            int strength = factory.DefaultStrength * percentsUpdate / 100;

            return factory.Create(position, health, agility, strength);
        }
    }

    private static Item GenerateItem(Player player)
    {
        Item.Factory factory = Item.Factories[Random.Shared.Next(Item.Factories.Length)];
        return factory.Generate(player);
    }

    private static void GenerateItems(Level level, Player player, Room playerRoom)
    {
        int maxItems = Constants.MaxConsumablesPerRoom - level.LevelNumber / Constants.LevelUpdateDifficulty;
        foreach (var room in level.Rooms)
        {
            if (room == playerRoom)
            {
                continue;
            }

            int numberOfItems = Random.Shared.Next(0, maxItems);
            for (int i = 0; i < numberOfItems; i++)
            {
                Vector position = GenerateFreeVector(level, room);
                Item item = GenerateItem(player);
                WorldItem worldItem = new(item, position);
                level.Objects.Add(worldItem);
            }
        }
    }

    private static void GenerateExit(Level level, Room playerRoom)
    {
        while (true)
        {
            int roomIndex = Random.Shared.Next(level.Rooms.Length);
            Room room = level.Rooms[roomIndex];
            if (room == playerRoom)
            {
                continue;
            }

            // Ensure the exit is not too close to walls
            Extent extent = new(room.InnerExtent.Position + Vector.One, room.InnerExtent.Size - Vector.One * 2);
            Vector exitPosition = extent.RandomVector();
            if (level.IsOccupied(exitPosition))
            {
                continue;
            }

            Exit exit = new(exitPosition);
            level.Objects.Add(exit);
            return;
        }
    }
}
