namespace Bot.Models;

public class Manufacturer : BaseEntity
{
    public virtual IEnumerable<Item> Items { get; set; }

    public string Name { get; set; }
}