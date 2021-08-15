using System.Text.RegularExpressions;
using RatingsBot.Models;
using Telegram.Bot.Types.InlineQueryResults;

namespace RatingsBot.Helpers
{
    public static class InlineQueryHelpers
    {
        public static InlineQueryResultArticle GetArticle(Rating rating)
        {
            new InlineQueryResultArticle(rating.Id, rating.Item.Name, new InputTextMessageContent(rating.Item.Name))
            {
                Description = "⭐"
            }
        }
    }
}