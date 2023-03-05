using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Authorization;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.Interfaces;

namespace OutOfTimePrototype.Controllers;

[Route("api/lecture-hall")]
[ApiController]
public class LectureHallController : ControllerBase
{
    private readonly ILectureHallService _lectureHallService;

    public LectureHallController(ILectureHallService lectureHallService)
    {
        _lectureHallService = lectureHallService;
    }

    /// <summary>
    ///     Method to obtain free (unoccupied) lecture halls on the specified date and time
    /// </summary>
    /// <param name="timeSlotNumber">The number of the class by time</param>
    /// <param name="date">The day on which free lectures are obtained</param>
    /// <remarks>This is a method for the bureau of schedules</remarks>
    [Authorize] [MinRoleAuthorize(Role.ScheduleBureau)]
    [HttpGet("free-halls")]
    public async Task<IActionResult> GetFreeHalls([FromQuery] int timeSlotNumber, [FromQuery] DateTime date)
    {
        var result = new
            { unoccupiedHalls = await _lectureHallService.GetAllUnoccupied(timeSlotNumber, date) };
        return Ok(result);
    }

    /// <summary>
    ///     Returns all lecture halls existing in the specified building
    /// </summary>
    /// <param name="buildingId">Guid of the building</param>
    /// <remarks>This is a method for casual users</remarks>
    [HttpGet]
    public async Task<IActionResult> GetHallsByBuildingId([FromQuery] Guid buildingId)
    {
        var result = await _lectureHallService.GetByBuilding(buildingId);
        return Ok(result);
    }

    [Authorize] [MinRoleAuthorize(Role.Admin)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateHall(LectureHallCreateDto lectureHallDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _lectureHallService.CreateLectureHall(lectureHallDto);
        return result.Match<IActionResult>(
            NoContent,
            e => e is AlreadyExistsException ? Conflict(e.Message) : StatusCode(500)
        );
    }

    [Authorize] [MinRoleAuthorize(Role.Admin)]
    [HttpPut("update")]
    public async Task<IActionResult> EditHall([FromQuery] Guid id, LectureHallUpdateDto hallUpdateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var r = await _lectureHallService.EditLectureHall(id, hallUpdateDto);
        return r.Match<IActionResult>(
            NoContent,
            e => e is RecordNotFoundException ? NotFound(e.Message) : StatusCode(500)
        );
    }
}