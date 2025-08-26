namespace Rogue.Domain;

public readonly record struct Extent(Vector Position, Vector Size)
{
    public static Extent Single(Vector position) => new(position, Vector.One);

    public bool Contains(Vector vector) =>
        vector.X >= Position.X && vector.X < Position.X + Size.X &&
        vector.Y >= Position.Y && vector.Y < Position.Y + Size.Y;

    public bool Contains(Extent extent) =>
        Contains(extent.Position) &&
        Contains(new Vector(extent.Position.X + extent.Size.X - 1, extent.Position.Y + extent.Size.Y - 1));

    public bool Intersects(Extent extent) =>
        Position.X < extent.Position.X + extent.Size.X &&
        Position.X + Size.X > extent.Position.X &&
        Position.Y < extent.Position.Y + extent.Size.Y &&
        Position.Y + Size.Y > extent.Position.Y;

    public Vector RandomVector() => new(
        Random.Shared.Next(Position.X, Position.X + Size.X),
        Random.Shared.Next(Position.Y, Position.Y + Size.Y));
}
