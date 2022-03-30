namespace Core.Models;

public record Manufacturer : BaseEntity
{
    public string Name { get; init; }

    public virtual IEnumerable<Item> Items { get; init; }
}