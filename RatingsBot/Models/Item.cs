namespace RatingsBot.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
