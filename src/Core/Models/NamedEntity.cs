namespace Core.Models;

public record NamedEntity : BaseEntity
{
    public string Name { get; init; }
}