using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Services;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    _ = logging.ClearProviders();
    _ = logging.AddConsole();
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IQuoteSource, QuoteService>();
builder.Services.AddSingleton<IStrategyService, StrategyPoolService>();
//builder.Services.AddSingleton<CentralizedBotService>();
builder.Services.AddHostedService<CentralizedBotService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
