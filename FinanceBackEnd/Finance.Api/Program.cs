using Finance.Api.Core.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDataBase(builder);

builder.Services.MainServices();

var app = builder.Build();

app.MainConfiguration();

if (app.Environment.IsDevelopment())
{
    app.ConfigureOpenApi();
}

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

app.Run();
