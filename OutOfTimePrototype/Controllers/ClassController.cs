using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using System.Web;
using System;
using static OutOfTimePrototype.Utilities.ClassUtilities.ClassOperationResult;
using System.Collections.Specialized;
using System.Security.Claims;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.Services.Interfaces;

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

        /// <summary>
        /// Sets the properties of a class with an Id passed in the route to match the ones specified in DTO.
        /// The Id itself cannot be edited and the corresponding DTO property is ignored.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="classEditDto"></param>
        /// <param name="nullMode">
        /// When true, all the unspecified (null) parameters passed in DTO are to be set null to the actual class (thus such requests with unspecified Date, TimeSlot or Cluster properties will fail).
        /// When false, null parameters are ignored.
        /// </param>
        /// <returns></returns>
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
            var result = await _classService.TryDeleteClass(id);

            return StatusCode(Convert.ToInt32(result.HttpStatusCode), result.Message);
        }

        /// <summary>
        /// Used to retrieve classes that meet the specified requirements.
        /// Returns a collection of classes that match ALL the individual properties that have been supplied.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="clusterNumber"></param>
        /// <param name="educatorId"></param>
        /// <param name="lectureHallId"></param>
        /// <remarks>
        /// Sample request which will try to retrieve classes set from 20.02.2023 through to 26.02.2023 for cluster "1337":
        /// 
        ///     /api/class/query?startDate=2023-02-20&amp;endDate=2023-02-26&amp;clusterNumber=1337
        /// 
        /// </remarks>
        /// <returns>A collection of classes that match ALL the individual properties that have been supplied.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpGet, Route("query")]
        public async Task<IActionResult> QueryClasses(
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

            if (result.Status is not OperationStatus.Success)
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
