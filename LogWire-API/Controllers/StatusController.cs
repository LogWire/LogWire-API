using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Client;
using Microsoft.AspNetCore.Mvc;

namespace LogWire.API.Controllers
{
    [ApiController]
    public class StatusController : ControllerBase
    {

        [HttpGet]
        [Route("/status/system")]
        public async Task<ActionResult> GetStatus()
        {
            var value = await StatusApiClient.GetSystemStatus("https://localhost:5001", "e29e7516-829a-4e07-9e0f-a7ad7b3f27ec");

            return Ok(value);
        }

    }

}
