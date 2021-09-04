using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
                var options = provider.GetRequiredService<IOptions<TelegramOptions>>().Value;

                return new TelegramBotClient(options.Token);
            });
        }
    }
}
