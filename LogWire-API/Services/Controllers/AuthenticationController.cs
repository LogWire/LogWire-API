using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.API.Data.Model;
using LogWire.API.Data.Repository;
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
        private IDataRepository<RefreshTokenEntry> _refreshTokenRepo;

        public AuthenticationController(IConfiguration config, IDataRepository<RefreshTokenEntry> refreshTokenRepo)
        {
            _configuration = config;
            _refreshTokenRepo = refreshTokenRepo;
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

                    var refreshToken = TokenUtils.GenerateRefreshToken();
                    
                    _refreshTokenRepo.Add(new RefreshTokenEntry
                    {
                        Token = refreshToken,
                        CreatedAt = DateTime.UtcNow,
                        UserId = Guid.Parse(value)
                    });

                    var accessToken = TokenUtils.GenerateJwtToken(value);

                    return Ok(new { UserId = value, RefreshToken = refreshToken, AccessToken = accessToken });
                }

                return BadRequest(new { Message = "Error with username or password." });

            }

            return BadRequest(new { Message = "Missing headers" });

        }

    }
}
