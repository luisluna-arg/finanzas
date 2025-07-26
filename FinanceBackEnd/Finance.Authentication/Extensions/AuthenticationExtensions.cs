using System.Security.Claims;
using Finance.Authentication.Authorization;
using Finance.Authentication.Options;
using Finance.Authentication.Services;
using Finance.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Finance.Authentication.Extensions;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Configures Auth0 authentication and authorization for the application.
    /// </summary>
    /// <param name="services">The IServiceCollection to add authentication and authorization services to.</param>
    /// <param name="configuration">The application configuration containing Auth0 settings.</param>
    public static void ConfigureAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Options = configuration.GetSection(Auth0Options.SectionName).Get<Auth0Options>()
            ?? throw new InvalidOperationException("Auth0 configuration is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{auth0Options.Domain}/";
            options.Audience = auth0Options.Audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        // Configure authorization policies with database access
        services.AddAuthorization(options =>
        {
            // Get DbContext from service provider at configuration time
            var sp = services.BuildServiceProvider();
            var dbContext = sp.GetRequiredService<FinanceDbContext>();

            // Configure policies with authorization options
            AuthorizationPolicyProvider.ConfigurePolicies(sp, options);
        });

        // Register Auth0 user validation service
        services.AddScoped<IAuth0UserValidationService, Auth0UserValidationService>();

        // Configure Auth0 options
        services.Configure<Auth0Options>(configuration.GetSection(Auth0Options.SectionName));

        // Configure Admin User options
        services.Configure<AdminUserOptions>(configuration.GetSection(AdminUserOptions.SectionName));
    }
}
