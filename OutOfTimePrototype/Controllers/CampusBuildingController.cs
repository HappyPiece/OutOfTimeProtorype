using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Authorization;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services;
using OutOfTimePrototype.Services.General.Interfaces;

namespace OutOfTimePrototype.Controllers;

[Route("api/campus-building")]
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

    [MinRoleAuthorize(Role.Admin)]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CampusBuildingDto campusBuildingDto)
    {
        var result = await _campusBuildingService.Create(campusBuildingDto);
        
        // TODO: make sure that all errors are returned in the ModelState format (now it's not)
        return result.Match<IActionResult>(
            NoContent,
            e =>
            {
                return e switch
                {
                    ValidationException => BadRequest(e.Message),
                    AlreadyExistsException => Conflict(e.Message),
                    RecordNotFoundException => NotFound(e.Message),
                    _ => StatusCode(500)
                };
            }
        );
    }

    [MinRoleAuthorize(Role.Admin)]
    [HttpPut("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] CampusBuildingDto campusBuildingDto)
    {
        var result = await _campusBuildingService.Edit(id, campusBuildingDto);
        return result.Match<IActionResult>(
            NoContent,
            e => e is RecordNotFoundException ? BadRequest(e.Message) : StatusCode(500)
        );
    }

    [MinRoleAuthorize(Role.Admin)]
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