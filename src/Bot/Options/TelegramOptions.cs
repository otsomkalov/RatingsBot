namespace Bot.Options;

public class TelegramOptions
{
    public const string SectionName = "Telegram";

    public string ApiUrl { get; set; }

    public string Token { get; set; }
}