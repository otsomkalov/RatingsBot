using Core.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;
var configuration = builder.Configuration;

services.AddApplicationInsightsTelemetry();

services.AddDbContext<AppDbContext>(optionsBuilder =>
    optionsBuilder.UseNpgsql(configuration.GetConnectionString(DatabaseOptions.ConnectionStringName)));

services.AddLocalization()
    .AddServices();

services.Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.SectionName))
    .AddTelegram();

services.AddMediatR(typeof(Program));

services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.MapControllers();

app.Run();