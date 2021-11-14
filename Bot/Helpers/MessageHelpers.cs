using RatingsBot.Models;

namespace RatingsBot.Helpers;

public static class MessageHelpers
{
    public static string GetItemMessageText(Item item, long userId, string messageTemplate)
    {
        var currentUserRating = item.Ratings.FirstOrDefault(r => r.UserId == userId);

        var avgRating = item.Ratings.Any()
            ? item.Ratings.Sum(r => r.Value) / item.Ratings.Count
            : 0;

        var currentRatingString = currentUserRating == null
            ? "No rating"
            : string.Join(string.Empty, Enumerable.Repeat("⭐", currentUserRating.Value));

        var avgRatingString = avgRating == 0
            ? "No average rating"
            : string.Join(string.Empty, Enumerable.Repeat("⭐", avgRating));

        var placeName = item.Place?.Name ?? "<None>";

        return string.Format(messageTemplate, item.Name, item.Category?.Name, placeName,
            currentRatingString, avgRatingString);
    }
}