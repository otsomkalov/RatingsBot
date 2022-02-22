namespace Bot.Models;

public class Item : BaseEntity
{
    public virtual Category Category { get; set; }

    public int? CategoryId { get; set; }

    public virtual Manufacturer Manufacturer { get; set; }

    public int? ManufacturerId { get; set; }

    public string Name { get; set; }

    public virtual Place Place { get; set; }

    public int? PlaceId { get; set; }

    public virtual IReadOnlyCollection<Rating> Ratings { get; set; }
}