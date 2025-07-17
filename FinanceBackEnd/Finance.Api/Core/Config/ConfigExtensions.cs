using CQRSDispatch;
using CQRSDispatch.Extensions;
using CQRSDispatch.Interfaces;
using Finance.Application.Extensions;
using Finance.Application.Mapping;
using Finance.Application.Repositories;
using Finance.Domain.DataConverters;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Finance.Api.Core.Config;

public static class ConfigExtensions
{
    public const string AllowOriginsForCORSPolicy = "AllowOriginsForCORSPolicy";

    public static void MainServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(AppModuleRepository).Assembly;

        // Register command and query handlers from Application assembly
        services.AddRequestHandlers([applicationAssembly]);

        services.AddScoped<IDispatcher, Dispatcher>();

        services.AddMappers();

        services.AddRepositories();

        services.AddEntityServices();

        services.AddSagaServices();

        services.AddScoped<ICurrencyConverter, CurrencyConverter>();

        services.AddCors(options =>
        {
            options.AddPolicy(
                AllowOriginsForCORSPolicy,
                policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        // Make sure API Explorer is enabled for controller discovery
        services.AddEndpointsApiExplorer();

        services.AddScoped<IOLInvestmentExcelHelper>();
    }

    public static void ConfigureDataBase(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Configure Auth0 options
        services.Configure<Authentication.Options.Auth0Options>(builder.Configuration.GetSection(Authentication.Options.Auth0Options.SectionName));

        // Configure Admin User options
        services.Configure<Authentication.Options.AdminUserOptions>(builder.Configuration.GetSection(Authentication.Options.AdminUserOptions.SectionName));

        // Add Context to dependency injection
        services.AddDbContext<FinanceDbContext>(opt =>
        {
            opt.UseLazyLoadingProxies(false);
            opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb"));
        });
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Configure controllers with Newtonsoft JSON
        services.AddControllers()
        .AddNewtonsoftJson(o =>
        {
            o.SerializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });
        });

        // Add the DatabaseSeeder as a hosted service
        services.AddHostedService<DatabaseSeeder>();
    }

    public static void MainConfiguration(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();

        // Important: Add CORS before authentication middleware
        app.UseCors(AllowOriginsForCORSPolicy);

        // Authentication and authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        // Configure controllers
        app.MapControllers();

        // Configure Swagger and API reference for both development and production
        SwaggerConfig.ConfigureOpenApiUI(app);
    }
}
