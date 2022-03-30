namespace Core.Models;

public record Category : BaseEntity
{
    public virtual IEnumerable<Item> Items { get; init; }

    public string Name { get; init; }
}