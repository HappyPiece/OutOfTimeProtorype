using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Services;
using System.Web;
using System;
using static OutOfTimePrototype.Utilities.ClassUtilities.ClassOperationResult;
using System.Collections.Specialized;
using System.Security.Claims;
using OutOfTimePrototype.Dto;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly IClassService _classService;

        public ClassController(OutOfTimeDbContext outOfTimeDbContext, IClassService classService)
        {
            _classService = classService;
            _outOfTimeDbContext = outOfTimeDbContext;
        }

        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateClass(ClassDto ClassDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _classService.TryCreateClass(ClassDto);
            return StatusCode(Convert.ToInt32(result.HttpStatusCode), result.Message);
        }

        [HttpPost, Route("{id}/edit")]
        public async Task<IActionResult> EditClass(Guid id, ClassEditDto classEditDto, [FromQuery] bool nullMode = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _classService.TryEditClass(id, classEditDto, nullMode);
            return StatusCode(Convert.ToInt32(result.HttpStatusCode), result.Message);
        }

        [HttpDelete, Route("{id}/delete")]
        public async Task<IActionResult> DeleteClass([FromRoute] Guid id)
        {
            Class? @class = await _outOfTimeDbContext.Classes.SingleOrDefaultAsync(x => x.Id == id);

            if (@class is null)
            {
                return NotFound();
            }

            _outOfTimeDbContext.Classes.Remove(@class);
            await _outOfTimeDbContext.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpGet, Route("get")]
        public async Task<IActionResult> GetClasses(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? clusterNumber,
            [FromQuery] Guid? educatorId,
            [FromQuery] Guid? lectureHallId
            )
        {
            ClassQueryDto classQueryDto = new ClassQueryDto
            {
                StartDate = startDate,
                EndDate = endDate,
                ClusterNumber = clusterNumber,
                EducatorId = educatorId,
                LectureHallId = lectureHallId
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _classService.QueryClasses(classQueryDto);

            if (result.Status is not ClassOperationStatus.Success)
            {
                return StatusCode(Convert.ToInt32(result.HttpStatusCode), result.Message);
            }

            if (result.QueryResult is null) throw new ArgumentNullException("Expected not null query result");

            return StatusCode(Convert.ToInt32(result.HttpStatusCode), result.QueryResult.Select(x => new ClassDto
            {
                Id = x.Id,
                Date = x.Date,
                TimeSlotNumber = x.TimeSlot.Number,
                ClusterNumber = x.Cluster.Number,
                EducatorId = x.Educator?.Id,
                LectureHallId = x.LectureHall?.Id,
                ClassTypeName = x.Type?.Name
            }));
        }

        [HttpGet, Route("{id}/get")]
        public async Task<IActionResult> GetClass([FromRoute] Guid id)
        {
            Class? @class = await _outOfTimeDbContext.Classes
                .Include(x => x.TimeSlot)
                .Include(x => x.Cluster)
                .Include(x => x.Educator)
                .Include(x => x.LectureHall)
                .Include(x => x.Type)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (@class is null)
            {
                return NotFound();
            }

            var result = new ClassDto
            {
                Id= @class.Id,
                Date = @class.Date,
                TimeSlotNumber = @class.TimeSlot.Number,
                ClusterNumber = @class.Cluster.Number,
                EducatorId = @class.Educator?.Id,
                LectureHallId = @class.LectureHall?.Id,
                ClassTypeName = @class.Type?.Name
            };

            return Ok(result);
        }
    }
}
