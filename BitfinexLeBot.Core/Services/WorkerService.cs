using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.FundingInfo;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services;

/// <summary>
/// All registered userStrategies are executed in a worker(single thread)
/// </summary>
public class WorkerService : BackgroundService
{
    private readonly ILogger<CentralizedBotService> _logger;

    private IBotService _botService;


    public WorkerService(ILogger<CentralizedBotService> logger, IBotService botService)
    {
        _logger = logger;
        _botService = botService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _botService.RunStep();
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
