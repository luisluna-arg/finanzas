using Finance.Api.Core.Config;
using Finance.Authentication.Extensions;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);
if (isDevelopment)
{
    DotNetEnv.Env.LoadMulti([".env.local"]);
}

var builder = WebApplication.CreateBuilder(args);

// Configure URLs from appsettings or environment variables
var httpUrl = Environment.GetEnvironmentVariable("Urls__Http");
if (string.IsNullOrWhiteSpace(httpUrl))
{
    throw new InvalidOperationException("HTTP URL is required. Please configure 'Urls:Http' in appsettings.json or set the 'Urls__Http' environment variable.");
}

var httpsUrl = Environment.GetEnvironmentVariable("Urls__Https");
if (!string.IsNullOrWhiteSpace(httpsUrl))
{
    // Configure both HTTP and HTTPS if HTTPS URL is provided
    builder.WebHost.UseUrls(httpUrl, httpsUrl);
}
else
{
    // Configure only HTTP if HTTPS URL is not provided
    builder.WebHost.UseUrls(httpUrl);
}

builder.Services.ConfigureDataBase(builder);

builder.Services.MainServices();

// Add Swagger with authentication
builder.Services.AddSwaggerWithAuth();

// Configure Auth0 authentication and authorization using the authentication extension
builder.Services.ConfigureAuth0Authentication(builder.Configuration);

var app = builder.Build();

// Configure the application with all necessary middleware
app.MainConfiguration();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

app.Run();
