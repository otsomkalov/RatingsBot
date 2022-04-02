using Bot.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helpers;

public static class ReplyKeyboardMarkupHelpers
{
    public static ReplyKeyboardMarkup GetStartReplyKeyboardMarkup()
    {
        var buttonsRows = new[]
        {
            new[]
            {
                new KeyboardButton(Commands.NewCategory)
            },
            new[]
            {
                new KeyboardButton(Commands.NewPlace)
            },
            new[]
            {
                new KeyboardButton(Commands.NewManufacturer)
            },
            new[]
            {
                new KeyboardButton(Commands.NewItem)
            }
        };

        return new(buttonsRows)
        {
            OneTimeKeyboard = true
        };
    }
}