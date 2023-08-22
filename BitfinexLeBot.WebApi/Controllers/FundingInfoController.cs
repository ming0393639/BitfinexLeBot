// Copyright 2023 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models.FundingInfo;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BitfinexLeBot.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FundingInfoController : ControllerBase
{
    private readonly ILogger<FundingInfoController> _logger;

    private readonly IBotService _botService;

    public FundingInfoController(ILogger<FundingInfoController> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _botService = serviceProvider.GetRequiredService<IBotService>();

    }



    // GET: api/<FundingInfoController>
    [HttpGet]
    public IEnumerable<FundingState> Get()
    {
        return new FundingState[] { };
    }

    // GET api/<FundingInfoController>/5
    [HttpGet("{id}")]
    public FundingState Get(int id)
    {
        var registerUserStrategyList = _botService.GetRegisteredUserStrategy();
        var userStrategy = registerUserStrategyList.Find(userStrategy => userStrategy.User.BotUserId == id);

        var state = _botService.GetFundingState(userStrategy.User, userStrategy.FundingSymbol);
        return state;
    }

    // POST api/<FundingInfoController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<FundingInfoController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<FundingInfoController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
