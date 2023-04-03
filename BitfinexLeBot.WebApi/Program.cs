using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Services;
using BitfinexLeBot.Core.Services.Quote;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
builder.Host.UseSerilog();

//builder.Host.ConfigureLogging((context, logging) =>
//{
//    logging.ClearProviders();
//});



// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IQuoteSource, NormalQuoteService>();
builder.Services.AddSingleton<IStrategyService, StrategyPoolService>();
builder.Services.AddSingleton<IBotService, CentralizedBotService>();
builder.Services.AddHostedService<WorkerService>();

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
