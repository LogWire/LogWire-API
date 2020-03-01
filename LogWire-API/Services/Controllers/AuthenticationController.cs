using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.API.Data.Model;
using LogWire.API.Data.Repository;
using LogWire.API.Utils;
using LogWire.Controller.Client;
using Microsoft.AspNetCore.Http;
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
                    HttpContext.Response.Cookies.Append("rt", EncryptionUtil.Encrypt(refreshToken, Startup.EncryptionKey),new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true
                    });

                    return Ok(new { AccessToken = accessToken, TokenExpires= 900000 });
                }

                return BadRequest(new { Message = "Error with username or password." });

            }

            return BadRequest(new { Message = "Missing headers" });

        }

        [HttpPost]
        [Route("/auth/refresh")]
        public async Task<ActionResult> Refresh()
        {

            var s = HttpContext.Request.Cookies["rt"];

            if (!String.IsNullOrWhiteSpace(s))
            {
                s = EncryptionUtil.Decrypt(s, Startup.EncryptionKey);

                var tokenData = _refreshTokenRepo.Get(s);

                if (tokenData != null)
                {

                    var refreshToken = TokenUtils.GenerateRefreshToken();

                    _refreshTokenRepo.Add(new RefreshTokenEntry
                    {
                        Token = refreshToken,
                        CreatedAt = DateTime.UtcNow,
                        UserId = tokenData.UserId
                    });

                    _refreshTokenRepo.Delete(tokenData);

                    var accessToken = TokenUtils.GenerateJwtToken(tokenData.UserId.ToString());
                    HttpContext.Response.Cookies.Delete("rt");

                    HttpContext.Response.Cookies.Append("rt", EncryptionUtil.Encrypt(refreshToken, Startup.EncryptionKey), new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true
                    });

                    return Ok(new { AccessToken = accessToken, TokenExpires = 900000 });

                }

                return Unauthorized(new { Message = "Invalid Token" });

            }

            return BadRequest(new { Message = "Missing inforation" });

        }

    }
}
