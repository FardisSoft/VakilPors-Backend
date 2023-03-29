using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VakilPors.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        //fake api
        [HttpGet]
        public async Task<IActionResult> Get(string text)
        {
            return Ok("Hello World!"+text);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            return Ok("Hello World!");
        }

    }
}