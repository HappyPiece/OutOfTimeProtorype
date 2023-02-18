using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;

        public ClassController(OutOfTimeDbContext outOfTimeDbContext)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
        }

        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateClass(ClassDto classDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            throw new NotImplementedException();
        }
    }
}
