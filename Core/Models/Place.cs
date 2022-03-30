namespace Core.Models;

public record Place : BaseEntity
{
    public virtual IEnumerable<Item> Items { get; init; }

    public string Name { get; init; }
}