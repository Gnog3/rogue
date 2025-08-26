namespace Rogue.Domain;

public sealed class Passage(Extent extent)
{
    public Extent Extent { get; } = extent;
}
