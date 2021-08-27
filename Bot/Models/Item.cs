using System.Collections.Generic;

namespace RatingsBot.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }

        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual IEnumerable<Rating> Ratings { get; set; }
    }
}
