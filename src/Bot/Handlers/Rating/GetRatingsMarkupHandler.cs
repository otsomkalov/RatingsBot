using Bot.Constants;
using Bot.Requests.Rating;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Rating;

public class GetRatingsMarkupHandler : RequestHandler<GetRatingsMarkup, InlineKeyboardMarkup>
{
    protected override InlineKeyboardMarkup Handle(GetRatingsMarkup request)
    {
        var itemId = request.ItemId;

        var firstRow = new InlineKeyboardButton[]
        {
            new("⭐")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.OneStar)
            },
            new("⭐⭐")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.TwoStars)
            },
            new("⭐⭐⭐")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.ThreeStars)
            }
        };

        var secondRow = new InlineKeyboardButton[]
        {
            new("⭐⭐⭐⭐")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.FourStars)
            },
            new("⭐⭐⭐⭐⭐")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.FiveStars)
            }
        };

        var thirdRow = new InlineKeyboardButton[]
        {
            new("Refresh")
            {
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, 0)
            }
        };

        var buttons = new[]
        {
            firstRow, secondRow, thirdRow
        };

        return new(buttons);
    }
}