using Finance.Api.Core.Config;

var builder = WebApplication.CreateBuilder(args);

// Explicitly set the URLs the application will listen on
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5100");

builder.Services.ConfigureDataBase(builder);

builder.Services.MainServices();

// Add Swagger with authentication
builder.Services.AddSwaggerWithAuth();

// Configure Auth0 authentication and authorization using the authentication extension
Finance.Authentication.Extensions.AuthenticationExtensions.ConfigureAuth0Authentication(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the application with all necessary middleware
app.MainConfiguration();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

app.Run();
