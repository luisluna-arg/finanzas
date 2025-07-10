using Finance.Api.Core.Options;
using Finance.Api.Core.Services;
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

        services.AddMediatR(o => o.RegisterServicesFromAssembly(applicationAssembly));

        services.AddMappers();

        services.AddRepositories();

        services.AddEntityServices();

        services.AddScoped<ICurrencyConverter, CurrencyConverter>();

        services.AddCors(options =>
        {
            options.AddPolicy(
                AllowOriginsForCORSPolicy,
                policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        services.AddScoped<IOLInvestmentExcelHelper>();

        // Add Auth0 user validation service
        services.AddScoped<IAuth0UserValidationService, Auth0UserValidationService>();
    }

    public static void ConfigureDataBase(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Configure Auth0 options
        services.Configure<Auth0Options>(builder.Configuration.GetSection(Auth0Options.SectionName));

        // Configure Admin User options
        services.Configure<AdminUserOptions>(builder.Configuration.GetSection(AdminUserOptions.SectionName));

        // Add Context to dependency injection
        services.AddDbContext<FinanceDbContext>(opt =>
        {
            opt.UseLazyLoadingProxies(false);
            opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb"));
        });
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddControllers().AddNewtonsoftJson(o =>
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
        // Only in production we'll protect the Scalar endpoint
        SwaggerConfig.ConfigureOpenApiUI(app);
    }
}
