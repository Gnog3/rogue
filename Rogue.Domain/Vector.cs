namespace Rogue.Domain;

public record struct Vector(int X, int Y)
{
    public static Vector One => new(1, 1);

    public static Vector operator +(Vector a, Vector b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Vector operator -(Vector a, Vector b)
        => new(a.X - b.X, a.Y - b.Y);

    public static Vector operator *(Vector a, int b)
        => new(a.X * b, a.Y * b);
}
