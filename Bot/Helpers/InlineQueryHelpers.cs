using System.Linq;
using RatingsBot.Models;
using Telegram.Bot.Types.InlineQueryResults;

namespace RatingsBot.Helpers
{
    public static class InlineQueryHelpers
    {
        public static InlineQueryResultArticle GetArticle(Rating rating)
        {
            return new(rating.Id, rating.Item.Name, new InputTextMessageContent(rating.Item.Name))
            {
                Description = string.Join(string.Empty, Enumerable.Repeat("⭐", rating.Value))
            };
        }
    }
}