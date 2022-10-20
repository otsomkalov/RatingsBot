using System.Reflection;
using Bot;
using Core.Data;
using Core.Requests.Category;
using Core.Requests.Item;
using Core.Requests.Manufacturer;
using Core.Requests.Place;
using Core.Validators.Category;
using Core.Validators.Item;
using Core.Validators.Manufacturer;
using Core.Validators.Place;
using Data;
using FluentValidation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Bot;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.GetContext().Configuration;

        services
            .Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.SectionName))
            .Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<AppDbContext>((provider, optionsBuilder) =>
        {
            var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

            optionsBuilder.UseNpgsql(options.ConnectionString);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddLocalization()
            .AddServices()
            .AddTelegram();

        services
            .AddSingleton<IValidator<CreateItem>, CreateItemValidator>()
            .AddSingleton<IValidator<CreateManufacturer>, CreateManufacturerValidator>()
            .AddSingleton<IValidator<CreateCategory>, CreateCategoryValidator>()
            .AddSingleton<IValidator<CreatePlace>, CreatePlaceValidator>()
            .AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

        services.AddServices();
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder.AddUserSecrets<Startup>(true);
    }
}