namespace Core.Models;

public record Category : BaseEntity
{
    public string Name { get; init; }

    public virtual IEnumerable<Item> Items { get; init; }
}