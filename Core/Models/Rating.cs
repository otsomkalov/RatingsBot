namespace Core.Models;

public record Rating : BaseEntity
{
    public int ItemId { get; init; }

    public long UserId { get; init; }

    public int Value { get; init; }

    public virtual Item Item { get; init; }

    public virtual User User { get; init; }
}