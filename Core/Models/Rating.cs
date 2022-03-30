namespace Core.Models;

public record Rating : BaseEntity
{
    public virtual Item Item { get; init; }

    public int ItemId { get; init; }

    public virtual User User { get; init; }

    public long UserId { get; init; }

    public int Value { get; init; }
}