namespace Core.Models;

public record Manufacturer : NamedEntity
{
    public virtual IEnumerable<Item> Items { get; init; }
}