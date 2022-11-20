// Based on Microsoft Minimal API tutorial
using FinanceApi.ApiMappings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Context to dependency injection
builder.Services.AddDbContext<FinanceDb>(opt => opt
    .UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Mappings();

app.Run();
