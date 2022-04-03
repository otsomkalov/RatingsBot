namespace Core.Models;

public record Place : NamedEntity
{
    public virtual IEnumerable<Item> Items { get; init; }
}