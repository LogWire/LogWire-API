using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Client.Clients;
using LogWire.Controller.Client.Clients.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LogWire.API.Services.Controllers
{
    [ApiController]
    public class SIEMController : ControllerBase
    {

        private IConfiguration _configuration;

        public SIEMController(IConfiguration config)
        {
            _configuration = config;
        }

        [Authorize]
        [HttpGet]
        [Route("/siem/list")]
        public async Task<ActionResult> ListApplications([FromQuery] int page, [FromQuery] int pageSize )
        {
            var value = await SIEMApiClient.ListUsers(_configuration["controller_endpoint"],
                _configuration["access_token"], page, pageSize);

            
            return Ok(value);
        }

    }
}