namespace Core.Models;

public record Place : BaseEntity
{
    public string Name { get; init; }

    public virtual IEnumerable<Item> Items { get; init; }
}