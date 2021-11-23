using Bot.Constants;
using Bot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helpers;

public static class ReplyMarkupHelpers
{
    public static InlineKeyboardMarkup GetRatingsMarkup(int itemId)
    {
        return new(new IEnumerable<InlineKeyboardButton>[]
        {
            new InlineKeyboardButton[]
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
            },
            new InlineKeyboardButton[]
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
            },
            new InlineKeyboardButton[]
            {
                new()
                {
                    Text = "Refresh",
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Rating, null)
                }
            }
        });
    }

    public static InlineKeyboardMarkup GetCategoriesMarkup(int itemId, IReadOnlyCollection<Category> categories)
    {
        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        for (var i = 0; i < categories.Count; i += ReplyMarkup.Columns)
        {
            rows.Add(categories.Skip(i)
                .Take(ReplyMarkup.Columns)
                .Select(c => new InlineKeyboardButton
                {
                    Text = c.Name,
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, c.Id)
                }));
        }

        rows.Add(new InlineKeyboardButton[]
        {
            new()
            {
                Text = "Refresh",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Category, null)
            }
        });

        return new(rows);
    }

    public static InlineKeyboardMarkup GetPlacesMarkup(int itemId, IReadOnlyCollection<Place> places)
    {
        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        for (var i = 0; i < places.Count; i += ReplyMarkup.Columns)
        {
            rows.Add(places.Skip(i)
                .Take(ReplyMarkup.Columns)
                .Select(place => new InlineKeyboardButton
                {
                    Text = place.Name,
                    CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Place, place.Id)
                }));
        }

        rows.Add(new InlineKeyboardButton[]
        {
            new()
            {
                Text = "<None>",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Place, null)
            },
            new()
            {
                Text = "Refresh",
                CallbackData = string.Join(ReplyMarkup.Separator, itemId, ReplyMarkup.Place, -1)
            }
        });

        return new(rows);
    }
}