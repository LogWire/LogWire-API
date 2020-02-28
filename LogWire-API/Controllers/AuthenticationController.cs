using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.API.Utils;
using LogWire.Controller.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LogWire.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private IConfiguration _configuration;

        public AuthenticationController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpPost]
        [Route("/auth/login")]
        public async Task<ActionResult> Login()
        {

            if (Request.HasHeader("username") && Request.HasHeader("password"))
            {

                var value = await AuthenticationApiClient.Login(_configuration["controller_endpoint"], _configuration["access_token"], Request.Headers["username"], Request.Headers["password"]);

                if (!String.IsNullOrWhiteSpace(value))
                {
                    return Ok(new { UserId = value });
                }

                return BadRequest(new { Message = "Error with username or password." });

            }

            return BadRequest(new { Message = "Missing headers" });

        }

    }
}
