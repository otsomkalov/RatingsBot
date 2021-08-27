namespace RatingsBot.Models
{
    public class Rating : BaseEntity
    {
        public int Value { get; set; }

        public string ItemId { get; set; }

        public long UsedId { get; set; }

        public virtual Item Item { get; set; }

        public virtual User User { get; set; }
    }
}