using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Authorization;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.Interfaces;

namespace OutOfTimePrototype.Controllers;

[Route("api/educator")]
[ApiController]
public class EducatorController : ControllerBase
{
    private readonly IEducatorService _educatorService;

    public EducatorController(IEducatorService educatorService)
    {
        _educatorService = educatorService;
    }

    /// <summary>
    /// Returns all educators
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = new { educators = await _educatorService.GetAll() };
        return Ok(result);
    }

    /// <summary>
    /// Method to obtain unoccupied lecture halls on the specified date and time
    /// </summary>
    /// <param name="timeSlotNumber">The number of the class by time</param>
    /// <param name="date">The day on which free lectures are obtained</param>
    /// <remarks>This is a method for the bureau of schedules</remarks>
    [Authorize] [MinRoleAuthorize(Role.ScheduleBureau)]
    [HttpGet("free_educ")]
    public async Task<IActionResult> GetUnoccupiedEducators([FromQuery] int timeSlotNumber, [FromQuery] DateTime date)
    {
        var result = new
            { unoccupiedEducators = await _educatorService.GetAllUnoccupied(timeSlotNumber, date) };
        return Ok(result);
    }

    [Authorize] [MinRoleAuthorize(Role.Admin)]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] EducatorDto educatorDto)
    {
        await _educatorService.Create(educatorDto);
        return NoContent();
    }

    [Authorize] [MinRoleAuthorize(Role.Admin)]
    [HttpPut("{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] EducatorDto educatorDto)
    {
        var result = await _educatorService.Edit(id, educatorDto);
        return result.Match<IActionResult>(
            NoContent,
            e => e is RecordNotFoundException ? BadRequest(e.Message) : StatusCode(500)
        );
    }

    [Authorize] [MinRoleAuthorize(Role.Admin)]
    [HttpDelete("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _educatorService.Delete(id);
        return result.Match<IActionResult>(
            NoContent,
            e => e is RecordNotFoundException ? BadRequest(e.Message) : StatusCode(500)
        );
    }
}