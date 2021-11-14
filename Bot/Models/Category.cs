namespace RatingsBot.Models;

public class Category : BaseEntity
{
    public string Name { get; set; }
        
    public virtual IEnumerable<Item> Items { get; set; }
}