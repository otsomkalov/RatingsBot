using Bot.Commands.Item;
using Bot.Resources;
using Microsoft.Extensions.Localization;

namespace Bot.Handlers.Item;

public class GetItemMessageTextHandler : RequestHandler<GetItemMessageText, string>
{
    private readonly IStringLocalizer<Messages> _localizer;

    public GetItemMessageTextHandler(IStringLocalizer<Messages> localizer)
    {
        _localizer = localizer;
    }

    protected override string Handle(GetItemMessageText request)
    {
        var item = request.Item;

        var avgRating = item.Ratings?.Any() == true
            ? item.Ratings.Sum(r => r.Value) / item.Ratings.Count
            : 0;

        var avgRatingString = avgRating == 0
            ? "No average rating"
            : StringHelpers.CreateStarsString(avgRating);

        var placeName = item.Place?.Name ?? string.Empty;
        var manufacturerName = item.Manufacturer?.Name ?? string.Empty;

        string ratingsString;

        if (item.Ratings?.Any() == true)
        {
            var ratings = new List<string>();

            foreach (var rating in item.Ratings)
            {
                var ratingLineText = string.Format(_localizer[nameof(Messages.RatingLineTemplate)], rating.User.FirstName,
                    StringHelpers.CreateStarsString(rating.Value));

                ratings.Add(ratingLineText);
            }

            ratingsString = string.Join(Environment.NewLine, ratings);
        }
        else
        {
            ratingsString = "No ratings";
        }

        return string.Format(_localizer[nameof(Messages.ItemMessageTemplate)], item.Category.Name, manufacturerName, item.Name, placeName,
            avgRatingString, ratingsString);
    }
}