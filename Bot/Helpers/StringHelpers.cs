namespace Bot.Helpers;

public class StringHelpers
{
    public static string CreateStarsString(int starsCount)
    {
        return string.Join(string.Empty, Enumerable.Repeat('⭐', starsCount));
    }
}