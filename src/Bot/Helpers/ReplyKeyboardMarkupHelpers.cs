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
                new KeyboardButton(Constants.Commands.NewCategory)
            },
            new[]
            {
                new KeyboardButton(Constants.Commands.NewPlace)
            },
            new[]
            {
                new KeyboardButton(Constants.Commands.NewManufacturer)
            },
            new[]
            {
                new KeyboardButton(Constants.Commands.NewItem)
            }
        };

        return new(buttonsRows)
        {
            OneTimeKeyboard = true
        };
    }
}