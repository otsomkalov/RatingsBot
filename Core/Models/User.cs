namespace Core.Models;

public class User
{
    public string FirstName { get; set; }

    public long Id { get; set; }

    public virtual IEnumerable<Rating> Ratings { get; set; }
}