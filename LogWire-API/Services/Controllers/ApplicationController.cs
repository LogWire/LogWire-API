using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.API.Models.Application;
using LogWire.Controller.Client;
using LogWire.Controller.Client.Clients.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LogWire.API.Services.Controllers
{
    [ApiController]
    public class ApplicationController : ControllerBase
    {

        private IConfiguration _configuration;

        public ApplicationController(IConfiguration config)
        {
            _configuration = config;
        }

        [Authorize]
        [HttpPut]
        [Route("/application")]
        public async Task<ActionResult> CreateApplication([FromBody] AddApplicationModel body)
        {
            if (string.IsNullOrWhiteSpace(body.Name))
            {
                return BadRequest(new {Message = "Name should not be empty"});
            }

            var value = await ApplicationApiClient.AddApplication(_configuration["controller_endpoint"], _configuration["access_token"], body.Name);
            return Ok(new { Id = value != null ? value.ToString() : "" });
        }

        [Authorize]
        [HttpGet]
        [Route("/application/list")]
        public async Task<ActionResult> ListApplications()
        {
            var value = await ApplicationApiClient.ListApplications(_configuration["controller_endpoint"],
                _configuration["access_token"]);

            return Ok(new { Applications = value });
        }
    }
}