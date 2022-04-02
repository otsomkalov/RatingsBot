using Core.Commands.Category;
using Core.Commands.Item;
using Core.Commands.Manufacturer;
using Core.Commands.Place;
using Core.Data;
using Core.Validators.Category;
using Core.Validators.Item;
using Core.Validators.Manufacturer;
using Core.Validators.Place;
using Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;
var configuration = builder.Configuration;

services.AddApplicationInsightsTelemetry();

services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(configuration.GetConnectionString(DatabaseOptions.ConnectionStringName));
    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

services.AddLocalization()
    .AddServices();

services
    .AddSingleton<IValidator<CreateItem>, CreateItemValidator>()
    .AddSingleton<IValidator<CreateManufacturer>, CreateManufacturerValidator>()
    .AddSingleton<IValidator<CreateCategory>, CreateCategoryValidator>()
    .AddSingleton<IValidator<CreatePlace>, CreatePlaceValidator>()
    .AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

services.Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.SectionName))
    .AddTelegram();

services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.MapControllers();

app.Run();