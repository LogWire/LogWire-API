using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Client;
using LogWire.Controller.Client.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace LogWire.API.Controllers
{
    [ApiController]
    public class StatusController : ControllerBase
    {

        private IConfiguration _configuration;

        public StatusController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpGet]
        [Route("/status/system")]
        public async Task<ActionResult> GetStatus()
        {
            var value = await StatusApiClient.GetSystemStatus(_configuration["controller_endpoint"], _configuration["access_token"]);

            return Ok(new { IsOk = value.Key, Message = value.Value });
        }

    }

}
