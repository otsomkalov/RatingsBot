using System.Collections.Generic;

namespace RatingsBot.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }

        public int? CategoryId { get; set; }

        public int? PlaceId { get; set; }

        public virtual Category Category { get; set; }

        public virtual Place Place { get; set; }

        public virtual IReadOnlyCollection<Rating> Ratings { get; set; }
    }
}
