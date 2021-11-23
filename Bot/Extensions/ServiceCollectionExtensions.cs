using Bot.Options;
using Bot.Services;
using Microsoft.Extensions.Options;

namespace Bot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegram(this IServiceCollection services)
    {
        return services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<TelegramOptions>>().Value;

            return new TelegramBotClient(options.Token, baseUrl: options.ApiUrl);
        });
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<CategoryService>()
            .AddScoped<ItemService>()
            .AddScoped<PlaceService>()
            .AddScoped<RatingService>()
            .AddScoped<UserService>()
            .AddScoped<MessageService>()
            .AddScoped<CallbackQueryService>()
            .AddScoped<InlineQueryService>();
    }
}