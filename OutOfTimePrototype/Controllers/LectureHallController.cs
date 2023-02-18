using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services;

namespace OutOfTimePrototype.Controllers;

[Route("api/lecture_hall")]
[ApiController]
public class LectureHallController : ControllerBase
{
    private readonly ILectureHallService _lectureHallService;

    public LectureHallController(ILectureHallService lectureHallService)
    {
        _lectureHallService = lectureHallService;
    }

    /// <summary>
    /// Method to obtain free (unoccupied) lecture halls on the specified date and time
    /// </summary>
    /// <param name="timeSlotNumber">The number of the class by time</param>
    /// <param name="date">The day on which free lectures are obtained</param>
    /// <remarks>This is a method for the bureau of schedules</remarks>
    [HttpGet("free_halls")]
    public async Task<IActionResult> GetFreeHalls([FromQuery] int timeSlotNumber, [FromQuery] DateTime date)
    {
        var result = new
            { freeHalls = await _lectureHallService.GetAllUnoccupied(timeSlotNumber, date) };

        return Ok(result);
    }

    /// <summary>
    /// Returns all lecture halls existing in the specified building
    /// </summary>
    /// <param name="buildingId">Guid of the building</param>
    /// <remarks>This is a method for casual users</remarks>
    [HttpGet]
    public async Task<IActionResult> GetHallsByBuildingId([FromQuery] Guid buildingId)
    {
        var result = await _lectureHallService.GetByBuilding(buildingId);

        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateHall(LectureHallDto lectureHallDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var r = await _lectureHallService.CreateLectureHall(lectureHallDto);
        if (r is { IsFailed: true, Error: AlreadyExistsException }) return Conflict(r.Error.Message);
        return NoContent();
    }

    [HttpPut("update")]
    public async Task<IActionResult> EditHall([FromQuery] Guid id, LectureHallUpdateModel hallUpdateModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var r = await _lectureHallService.EditLectureHall(id, hallUpdateModel);
        if (r is { IsFailed: true, Error: RecordNotFoundException }) return NotFound(r.Error.Message);
        return NoContent();
    }
}