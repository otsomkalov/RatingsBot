namespace Core.Models;

public record Category : NamedEntity
{
    public virtual IEnumerable<Item> Items { get; init; }
}