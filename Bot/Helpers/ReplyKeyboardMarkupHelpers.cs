using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helpers;

public static class ReplyKeyboardMarkupHelpers
{
    public static ReplyKeyboardMarkup GetStartReplyKeyboardMarkup()
    {
        return new()
        {
            Keyboard = new[]
            {
                new[]
                {
                    new KeyboardButton
                    {
                        Text = Constants.Commands.NewCategory
                    }
                },
                new[]
                {
                    new KeyboardButton
                    {
                        Text = Constants.Commands.NewPlace
                    }
                },
                new[]
                {
                    new KeyboardButton
                    {
                        Text = Constants.Commands.NewItem
                    }
                }
            },
            OneTimeKeyboard = true
        };
    }
}