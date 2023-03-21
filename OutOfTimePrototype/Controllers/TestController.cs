using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] DateTime date, DayOfWeek targetDay)
        {
            return Ok(date.GetDayOfWeekFromThisWeek(targetDay));
        }
    }
}
