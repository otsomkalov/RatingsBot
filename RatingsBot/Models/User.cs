using System.Collections.Generic;

namespace RatingsBot.Models
{
    public class User
    {
        public long Id { get; set; }

        public virtual IEnumerable<Rating> Ratings { get; set; }
    }
}
