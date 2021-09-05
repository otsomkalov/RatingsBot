using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using RatingsBot.Constants;
using RatingsBot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace RatingsBot.Helpers
{
    public static class ReplyMarkupHelpers
    {
        public static InlineKeyboardMarkup GetRatingsMarkup(int itemId)
        {
            return new(new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = "⭐",
                        CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.OneStar)
                    },
                    new InlineKeyboardButton
                    {
                        Text = "⭐⭐",
                        CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.TwoStars)
                    },
                    new InlineKeyboardButton
                    {
                        Text = "⭐⭐⭐",
                        CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.ThreeStars)
                    }
                },
                new []
                {
                    new InlineKeyboardButton
                    {
                        Text = "⭐⭐⭐⭐",
                        CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.FourStars)
                    },
                    new InlineKeyboardButton
                    {
                        Text = "⭐⭐⭐⭐⭐",
                        CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, RatingValues.FiveStars)
                    }
                }
            });
        }

        public static InlineKeyboardMarkup GetCategoriesMarkup(int itemId, IReadOnlyCollection<Category> categories)
        {
            var rows = new List<IEnumerable<InlineKeyboardButton>>();

            for (var i = 0; i < categories.Count; i+= ReplyMarkup.Columns)
            {
                rows.Add(categories.Skip(i).Take(ReplyMarkup.Columns).Select(c => new InlineKeyboardButton
                {
                    Text = c.Name,
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, c.Id)
                }));
            }

            return new(rows);
        }

        public static InlineKeyboardMarkup GetPlacesMarkup(string itemId, IReadOnlyCollection<Place> places)
        {
            var rows = new List<IEnumerable<InlineKeyboardButton>>();

            for (var i = 0; i < places.Count; i += ReplyMarkup.Columns)
            {
                rows.Add(places.Skip(i).Take(ReplyMarkup.Columns).Select(place => new InlineKeyboardButton
                {
                    Text = place.Name,
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Place, place.Id)
                }));
            }

            return new(rows);
        }

        public static InlineKeyboardMarkup GetUsersMarkup(string itemId, IReadOnlyCollection<User> users)
        {
            var rows = new List<IEnumerable<InlineKeyboardButton>>();

            for (var i = 0; i < users.Count; i+= ReplyMarkup.Columns)
            {
                rows.Add(users.Skip(i).Take(ReplyMarkup.Columns).Select(user => new InlineKeyboardButton
                {
                    Text = user.FirstName,
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.User, user.Id)
                }));
            }

            return new(rows);
        }
    }
}