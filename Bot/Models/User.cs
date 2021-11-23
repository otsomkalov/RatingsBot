namespace Bot.Models;

public class User
{
    public long Id { get; set; }

    public string FirstName { get; set; }

    public virtual IEnumerable<Rating> Ratings { get; set; }
}