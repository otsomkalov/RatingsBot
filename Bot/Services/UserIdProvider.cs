namespace Bot.Services;

public class UserIdProvider
{
    private readonly ISet<long> _ids = new HashSet<long>();

    public bool UserExists(long id)
    {
        return _ids.Contains(id);
    }

    public void AddUserId(long id)
    {
        _ids.Add(id);
    }
}