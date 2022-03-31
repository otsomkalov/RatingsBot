using Core.Services.Interfaces;

namespace Core.Services;

public class UserIdProvider : IUserIdProvider
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