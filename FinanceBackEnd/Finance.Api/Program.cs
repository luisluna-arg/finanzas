using Finance.Api.Core.Config;
using Finance.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs from appsettings or environment variables
var httpUrl = builder.Configuration["Urls:Http"];
var httpsUrl = builder.Configuration["Urls:Https"];

if (string.IsNullOrEmpty(httpUrl))
    throw new InvalidOperationException("HTTP URL is required. Please configure 'Urls:Http' in appsettings.json or set the 'Urls__Http' environment variable.");

if (string.IsNullOrEmpty(httpsUrl))
    throw new InvalidOperationException("HTTPS URL is required. Please configure 'Urls:Https' in appsettings.json or set the 'Urls__Https' environment variable.");

builder.WebHost.UseUrls(httpUrl, httpsUrl);

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
