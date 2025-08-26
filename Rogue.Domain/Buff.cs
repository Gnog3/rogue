using Rogue.Domain.Items;

namespace Rogue.Domain;

public sealed class Buff(Elixir elixir, TimeSpan usedAt)
{
    public Elixir Elixir { get; } = elixir;

    public bool IsExpired(TimeSpan now) => now >= usedAt + Elixir.Duration;
}
