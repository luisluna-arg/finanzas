// Based on Microsoft Minimal API tutorial
using FinanceApi.Core.Config;
using FinanceApi.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string allowOriginsForCORSPolicy = "_allowOriginsForCORSPolicy";

// Add Context to dependency injection
builder.Services
    .AddDbContextPool<FinanceDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.MainServices();
builder.Services.ConfigureNSwag();
builder.Services.AddAutoMapping();

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
