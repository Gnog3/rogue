namespace Rogue.Domain;

public enum Direction
{
    Forward,
    Back,
    Left,
    Right,
    DiagonallyForwardLeft,
    DiagonallyForwardRight,
    DiagonallyBackLeft,
    DiagonallyBackRight,
    Stop,
}

public static class DirectionExtensions
{
    public static Vector Vector(this Direction direction) => direction switch
    {
        Direction.Forward => new(0, -1),
        Direction.Back => new(0, 1),
        Direction.Left => new(-1, 0),
        Direction.Right => new(1, 0),
        Direction.DiagonallyForwardLeft => new(-1, -1),
        Direction.DiagonallyForwardRight => new(1, -1),
        Direction.DiagonallyBackLeft => new(-1, 1),
        Direction.DiagonallyBackRight => new(1, 1),
        Direction.Stop => new(0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}

public static class DirectionHelper
{
    public static Direction RandomSimple() => (Direction)Random.Shared.Next(4);
    public static Direction RandomDiagonal() => (Direction)Random.Shared.Next(4, 8);
    public static Direction RandomAll() => (Direction)Random.Shared.Next(8);

    public static IReadOnlyList<Direction> SimpleDirections { get; } =
    [
        Direction.Forward,
        Direction.Back,
        Direction.Left,
        Direction.Right,
    ];
}
