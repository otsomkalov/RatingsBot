using Microsoft.Extensions.DependencyInjection;
using RatingsBot.Options;
using Telegram.Bot;

namespace RatingsBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelegram(this IServiceCollection services)
        {
            return services.AddSingleton<ITelegramBotClient>(provider =>
            {
                var options = provider.GetRequiredService<TelegramOptions>();

                return new TelegramBotClient(options.Token);
            });
        }
    }
}
