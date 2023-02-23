using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] DayOfWeek dayOfWeek, DateTime startDate, DateTime endDate)
        {
            return Ok(dayOfWeek.GetDayOfWeekFromBetweenDates(startDate, endDate));
        }
    }
}
