using Bot.Commands.Rating;
using Bot.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Handlers.Rating;

public class GetRatingsMarkupHandler : RequestHandler<GetRatingsMarkup, InlineKeyboardMarkup>
{
    protected override InlineKeyboardMarkup Handle(GetRatingsMarkup request)
    {
        var itemId = request.ItemId;

        var firstRow = new InlineKeyboardButton[]
        {
            new()
            {
                Text = "⭐",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.OneStar)
            },
            new()
            {
                Text = "⭐⭐",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.TwoStars)
            },
            new()
            {
                Text = "⭐⭐⭐",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.ThreeStars)
            }
        };

        var secondRow = new InlineKeyboardButton[]
        {
            new()
            {
                Text = "⭐⭐⭐⭐",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.FourStars)
            },
            new()
            {
                Text = "⭐⭐⭐⭐⭐",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.FiveStars)
            }
        };

        var thirdRow = new InlineKeyboardButton[]
        {
            new()
            {
                Text = "Refresh",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, null)
            }
        };

        var buttons = new[]
        {
            firstRow, secondRow, thirdRow
        };

        return new(buttons);
    }
}