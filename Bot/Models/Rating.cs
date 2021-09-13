namespace RatingsBot.Models
{
    public class Rating : BaseEntity
    {
        public int Value { get; set; }

        public int ItemId { get; set; }

        public long UserId { get; set; }

        public virtual Item Item { get; set; }

        public virtual User User { get; set; }
    }
}