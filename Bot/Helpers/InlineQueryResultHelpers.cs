using Bot.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace Bot.Helpers;

public static class InlineQueryResultHelpers
{
    public static InlineQueryResultArticle GetItemQueryResult(Item item, InlineQuery inlineQuery, string messageTemplate,
        string ratingsTemplate)
    {
        var placeName = item.Place?.Name ?? string.Empty;
        var ratings = item.Ratings;
        var currentUserRating = ratings.FirstOrDefault(r => r.UserId == inlineQuery.From.Id);

        var avgRating = ratings.Any()
            ? CalculateAverageRating(ratings)
            : 0;

        var currentRatingString = currentUserRating != null
            ? StringHelpers.CreateStarsString(currentUserRating.Value)
            : "-";

        var avgRatingString = avgRating != 0
            ? StringHelpers.CreateStarsString(avgRating)
            : "-";

        var title = $"{item.Category?.Name} {item.Name} {placeName}";
        var messageText = MessageHelpers.GetItemMessageText(item, messageTemplate, ratingsTemplate, placeName, avgRatingString);
        var messageContent = new InputTextMessageContent(messageText);

        return new(item.Id.ToString(), title, messageContent)
        {
            Description = $"{currentRatingString}/{avgRatingString}",
            ReplyMarkup = ReplyMarkupHelpers.GetRatingsMarkup(item.Id)
        };
    }

    private static int CalculateAverageRating(IReadOnlyCollection<Rating> ratings)
    {
        return (int)Math.Round(ratings.Sum(r => r.Value) / (double)ratings.Count, MidpointRounding.ToPositiveInfinity);
    }
}