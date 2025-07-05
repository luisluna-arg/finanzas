using Finance.Application.Extensions;
using Finance.Application.Mapping;
using Finance.Application.Repositories;
using Finance.Domain.DataConverters;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Scalar.AspNetCore;

namespace Finance.Api.Core.Config;

public static class ConfigExtensions
{
    public const string AllowOriginsForCORSPolicy = "AllowOriginsForCORSPolicy";

    public static void MainServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(AppModuleRepository).Assembly;

        services.AddOpenApi();

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
    }

    public static void ConfigureDataBase(this IServiceCollection services, WebApplicationBuilder builder)
    {
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
        app.UseAuthorization();

        app.UseCors(AllowOriginsForCORSPolicy);

        app.MapControllers();
    }

    public static void ConfigureOpenApi(this WebApplication app)
    {
        app.MapOpenApi();

        app.UseSwaggerUI(opts =>
        {
            opts.SwaggerEndpoint("/openapi/v1.json", "Finances API v1");
        });

        app.MapScalarApiReference();
    }
}
