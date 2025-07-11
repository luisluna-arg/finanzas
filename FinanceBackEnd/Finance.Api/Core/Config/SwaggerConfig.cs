#pragma warning disable IDE0005 // Using directive is unnecessary
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerUI;
#pragma warning restore IDE0005

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
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Finances API",
                Version = "v1",
                Description = "API for managing financial data",
                Contact = new OpenApiContact
                {
                    Name = "Finance API Team"
                }
            });

            // Ensure all controllers are included in the documentation
            c.EnableAnnotations();

            // Include XML comments if they exist
            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
            foreach (var xmlFile in xmlFiles)
            {
                c.IncludeXmlComments(xmlFile);
            }

            // Use controller name as the tag for grouping
            c.TagActionsBy(api =>
            {
                if (api.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor controllerActionDescriptor)
                {
                    // Use the controller name without the "Controller" suffix
                    var controllerName = controllerActionDescriptor.ControllerName;
                    return new[] { controllerName };
                }

                return new[] { "Other" };
            });

            c.DocInclusionPredicate((name, api) => true);

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

    // Removed ConfigureAuth0Authentication method as it's now called directly from Program.cs
    // using the Finance.Authentication.Extensions.AuthenticationExtensions class

    /// <summary>
    /// Configures OpenAPI/Swagger for the application.
    /// </summary>
    /// <param name="app">The WebApplication to configure OpenAPI/Swagger UI for.</param>
    public static void ConfigureOpenApiUI(this WebApplication app)
    {
        // Use the Swashbuckle version of UseSwagger - only call once
        SwaggerBuilderExtensions.UseSwagger(app);

        // Configure Swagger UI with authentication
        app.UseSwaggerUI(opts =>
        {
            // Point to the Swagger JSON endpoint
            opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Finances API v1");

            // Make sure the route prefix is set to "swagger"
            opts.RoutePrefix = "swagger";

            // Configure Auth0 implicit flow for Swagger UI
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

        // Configure Scalar with the path to the API reference
        // The Scalar package should automatically find the Swagger JSON endpoint
        var scalarEndpoint = app.MapScalarApiReference("/api-reference");

        // Add a mechanism to check if the Swagger JSON is generating properly
        // This should help diagnose why Scalar isn't showing any endpoints

        // Configure a diagnostic endpoint to check that Swagger JSON is generated correctly
        app.MapGet("/api/swagger-json", async (HttpContext context) =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("<html><head><title>Swagger JSON</title></head><body>");
            await context.Response.WriteAsync("<h1>Swagger JSON Diagnostic</h1>");
            await context.Response.WriteAsync("<p>Check if the Swagger JSON endpoint is accessible:</p>");
            await context.Response.WriteAsync("<a href=\"/swagger/v1/swagger.json\" target=\"_blank\">View Swagger JSON</a>");
            await context.Response.WriteAsync("</body></html>");
        });

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
