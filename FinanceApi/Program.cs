// Based on Microsoft Minimal API tutorial
using FinanceApi.ApiMappings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string allowOriginsForCORSPolicy = "_allowOriginsForCORSPolicy";

// Add Context to dependency injection
builder.Services.AddDbContext<FinanceDb>(opt => opt
    .UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        allowOriginsForCORSPolicy,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseCors(allowOriginsForCORSPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Mappings();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

app.Run();
