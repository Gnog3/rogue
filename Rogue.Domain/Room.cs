namespace Rogue.Domain;

public sealed class Room(Extent extent)
{
    public Extent Extent { get; } = extent;

    public Extent InnerExtent => new(Extent.Position + Vector.One, Extent.Size - Vector.One * 2);

    public bool IsInside(Vector vector) => InnerExtent.Contains(vector);

    public bool IsInside(Extent extent) => InnerExtent.Contains(extent);
}
