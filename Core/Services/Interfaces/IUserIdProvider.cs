namespace Core.Services.Interfaces;

public interface IUserIdProvider
{
    bool UserExists(long id);
    void AddUserId(long id);
}