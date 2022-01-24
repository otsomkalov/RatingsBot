using Bot.Models;

namespace Bot.Helpers;

public static class MessageHelpers
{
    public static string GetItemMessageText(Item item, string messageTemplate, string ratingLineTemplate)
    {
        var avgRating = item.Ratings.Any()
            ? item.Ratings.Sum(r => r.Value) / item.Ratings.Count
            : 0;

        var avgRatingString = avgRating == 0
            ? "No average rating"
            : StringHelpers.CreateStarsString(avgRating);

        var placeName = item.Place?.Name ?? string.Empty;

        return GetItemMessageText(item, messageTemplate, ratingLineTemplate, placeName, avgRatingString);
    }

    public static string GetItemMessageText(Item item, string messageTemplate, string ratingLineTemplate, string placeName,
        string avgRatingString)
    {
        var ratings = new List<string>();

        foreach (var rating in item.Ratings)
        {
            var ratingLineText = string.Format(ratingLineTemplate, rating.User?.FirstName, StringHelpers.CreateStarsString(rating.Value));
            ratings.Add(ratingLineText);
        }

        var ratingsString = ratings.Any()
            ? string.Join(Environment.NewLine, ratings)
            : "No ratings";

        return string.Format(messageTemplate, item.Category?.Name, item.Name, placeName, avgRatingString, ratingsString);
    }
}