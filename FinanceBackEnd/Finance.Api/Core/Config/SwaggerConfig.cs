using System.Security.Claims;
using Finance.Api.Core.Authorization;
using Finance.Api.Core.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

// Disable StyleCop spacing rules for collection initializers in this file
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1026 // Code should not contain space after new or stackalloc keyword in implicitly typed array allocation

namespace Finance.Api.Core.Config;

public static class SwaggerConfig
{
    /// <summary>
    /// Adds OpenAPI/Swagger services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add Swagger services to.</param>
    /// <returns>The IServiceCollection with Swagger services added.</returns>
    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
    {
        // Get Auth0 configuration
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var domain = configuration["Auth0:Domain"] ?? "your-auth0-domain.auth0.com";

        // Add OpenAPI/Swagger services with authentication
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Finances API", Version = "v1" });

            // Audience will be provided in additional parameters

            // Add OAuth security definition to enable Auth0 authentication flow
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"https://{domain}/authorize"),
                        TokenUrl = new Uri($"https://{domain}/oauth/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "Open ID" },
                            { "profile", "Profile" },
                            { "email", "Email" }
                        }
                    }
                },
                Description = "Auth0 Authentication"
            });

            // Add global security requirement
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    ["openid", "profile", "email"]
                }
            });
        });

        // Configure Scalar API explorer
        services.AddEndpointsApiExplorer();

        // Add HttpContextAccessor for request information
        services.AddHttpContextAccessor();

        return services;
    }

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
            var dbContext = sp.GetRequiredService<Finance.Persistance.FinanceDbContext>();

            // ^ Will throw InvalidOperationException if DbContext is not available

            // Configure policies with database context
            AuthorizationPolicyProvider.ConfigurePoliciesWithDb(options, dbContext);
        });
    }

    /// <summary>
    /// Configures OpenAPI/Swagger for the application.
    /// </summary>
    /// <param name="app">The WebApplication to configure OpenAPI/Swagger UI for.</param>
    public static void ConfigureOpenApiUI(this WebApplication app)
    {
        // Use the Swashbuckle version of UseSwagger
        SwaggerBuilderExtensions.UseSwagger(app);

        // Add SwaggerUI
        _ = app.UseSwaggerUI(opts =>
        {
            // Point to the Swagger JSON endpoint
            opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Finances API v1");

            // Configure Auth0 implicit flow for Swagger UI - simpler approach
            var clientId = app.Configuration["Auth0:Application:ClientId"];
            var domain = app.Configuration["Auth0:Domain"];
            var audience = app.Configuration["Auth0:Audience"];

            // Only configure OAuth if we have the required parameters
            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(domain))
            {
                // Get client secret from configuration
                var clientSecret = app.Configuration["Auth0:Application:ClientSecret"];

                // Set OAuth2 flow properties
                opts.OAuthClientId(clientId);
                opts.OAuthClientSecret(clientSecret); // Include the client secret from config
                opts.OAuthRealm(domain);
                opts.OAuthAppName("Finances API - Swagger");

                // Set redirect URL for the OAuth flow

                // Try to determine the actual host and scheme from the request if available
                var request = app.Services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Request;

                // Default values
                string scheme = "https";
                string host = app.Environment.IsDevelopment() ? "localhost:7000" : (app.Configuration["ApplicationUrl"] ?? "localhost");

                // Override with actual values if available
                if (request != null && request.Host.HasValue)
                {
                    scheme = request.Scheme ?? scheme;
                    host = request.Host.Value;
                }

                // Allow explicit configuration override
                if (app.Environment.IsDevelopment())
                {
                    var configScheme = app.Configuration["Swagger:Scheme"];
                    if (!string.IsNullOrEmpty(configScheme))
                    {
                        scheme = configScheme;
                    }

                    var configHost = app.Configuration["Swagger:Host"];
                    if (!string.IsNullOrEmpty(configHost))
                    {
                        host = configHost;
                    }
                }

                // Make sure this matches the redirect URL registered in Auth0
                string redirectUrl = $"{scheme}://{host}/swagger/oauth2-redirect.html";

                // Use authorization code flow with PKCE - configure client ID and secret
                opts.OAuthConfigObject = new Swashbuckle.AspNetCore.SwaggerUI.OAuthConfigObject
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret, // Include the client secret in the config object
                    AppName = "Finances API - Swagger",
                    Scopes = ["openid", "profile", "email"],
                    UsePkceWithAuthorizationCodeGrant = true // Try to enable PKCE if supported
                };

                // Explicitly set OAuth URLs since they aren't available in OAuthConfigObject
                opts.OAuth2RedirectUrl(redirectUrl);

                // Additional parameters for Auth0
                var additionalParams = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(audience))
                {
                    additionalParams["audience"] = audience;
                }

                // Do NOT set response_type here as it's already set by UsePkceWithAuthorizationCodeGrant
                // The error "response_type was provided more than once" happens when we set it twice

                // Apply the additional parameters
                opts.OAuthAdditionalQueryStringParams(additionalParams);
            }
        });

        // Configure Scalar with authentication - Map it to a specific endpoint
        var scalarEndpoint = app.MapScalarApiReference("/api-reference");

        // Make Scalar UI accessible from the root path as well
        app.MapGet("/", context =>
        {
            context.Response.Redirect("/api-reference");
            return Task.CompletedTask;
        });

        // Do not require authorization for Scalar during development to make it easier to access
        if (!app.Environment.IsDevelopment())
        {
            scalarEndpoint.RequireAuthorization();
        }
    }
}
