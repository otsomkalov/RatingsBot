using Bot.Middlewares;
using Microsoft.EntityFrameworkCore;

namespace Bot;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry();

        services.AddDbContext<AppDbContext>(builder =>
            builder.UseNpgsql(_configuration.GetConnectionString(DatabaseOptions.ConnectionStringName)));

        services.AddLocalization()
            .AddServices();

        services.AddScoped<ExceptionHandlerMiddleware>();

        services.Configure<TelegramOptions>(_configuration.GetSection(TelegramOptions.SectionName))
            .AddTelegram();

        services.AddMediatR(typeof(Startup));

        services.AddControllers()
            .AddNewtonsoftJson();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<ExceptionHandlerMiddleware>();

        // app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}