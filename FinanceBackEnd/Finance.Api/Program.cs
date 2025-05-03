// Based on Microsoft Minimal API tutorial
using System.Reflection;
using Finance.Api.Core.Config;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
const string allowOriginsForCORSPolicy = "_allowOriginsForCORSPolicy";

// Add Context to dependency injection
builder.Services
    .AddDbContext<FinanceDbContext>(opt =>
        opt
            .UseLazyLoadingProxies(false)
            .UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.Converters.Add(new StringEnumConverter
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    });
});

builder.Services.AddSwaggerGen(c =>
{
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.MainServices();
builder.Services.ConfigureNSwag();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        allowOriginsForCORSPolicy,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

// Add the DatabaseSeeder as a hosted service
builder.Services.AddHostedService<DatabaseSeeder>();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

app.UseCors(allowOriginsForCORSPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();

app.MapControllers();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

app.Run();
