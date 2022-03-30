namespace Core.Models;

public record Manufacturer : BaseEntity
{
    public virtual IEnumerable<Item> Items { get; init; }

    public string Name { get; init; }
}