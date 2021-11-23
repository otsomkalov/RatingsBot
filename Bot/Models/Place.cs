namespace Bot.Models;

public class Place : BaseEntity
{
    public string Name { get; set; }

    public virtual IEnumerable<Item> Items { get; set; }
}