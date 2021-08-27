using RatingsBot.Constants;
using RatingsBot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace RatingsBot.Helpers
{
    public static class ReplyMarkupHelpers
    {
        public static InlineKeyboardMarkup GetRatingsMarkup(string itemId)
        {
            return new(new[]
            {
                new InlineKeyboardButton
                {
                    Text = "⭐",
                    CallbackData = $"{itemId}|{RatingValues.OneStar}"
                },
                new InlineKeyboardButton
                {
                    Text = "⭐⭐",
                    CallbackData = $"{itemId}|{RatingValues.TwoStars}"
                },
                new InlineKeyboardButton
                {
                    Text = "⭐⭐⭐",
                    CallbackData = $"{itemId}|{RatingValues.ThreeStars}"
                },
                new InlineKeyboardButton
                {
                    Text = "⭐⭐⭐⭐",
                    CallbackData = $"{itemId}|{RatingValues.FourStars}"
                },
                new InlineKeyboardButton
                {
                    Text = "⭐⭐⭐⭐⭐",
                    CallbackData = $"{itemId}|{RatingValues.FiveStars}"
                }
            });
        }
    }
}