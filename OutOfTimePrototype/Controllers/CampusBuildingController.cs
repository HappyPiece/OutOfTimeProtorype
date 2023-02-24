using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services;

namespace OutOfTimePrototype.Controllers;

[Route("api/campus_building")]
[ApiController]
public class CampusBuildingController : ControllerBase
{
    private readonly ICampusBuildingService _campusBuildingService;

    public CampusBuildingController(ICampusBuildingService campusBuildingService)
    {
        _campusBuildingService = campusBuildingService;
    }

    /// <summary>
    /// Returns all campus buildings
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = new { campusBuildings = await _campusBuildingService.GetAll() };
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CampusBuildingDto campusBuildingDto)
    {
        await _campusBuildingService.Create(campusBuildingDto);
        return NoContent();
    }

    [HttpPut("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] CampusBuildingDto campusBuildingDto)
    {
        var result = await _campusBuildingService.Edit(id, campusBuildingDto);
        return result.Match<IActionResult>(
            NoContent,
            e => e is RecordNotFoundException ? BadRequest(e.Message) : StatusCode(500)
        );
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _campusBuildingService.Delete(id);
        return result.Match<IActionResult>(
            NoContent,
            e => e is RecordNotFoundException ? BadRequest(e.Message) : StatusCode(500)
        );
    }
}