using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfTimePrototype.Authorization;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.General.Interfaces;

namespace OutOfTimePrototype.Controllers;

[Route("api/subject")]
[ApiController]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;
    private readonly IMapper _mapper;

    public SubjectController(ISubjectService subjectService, IMapper mapper)
    {
        _subjectService = subjectService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subjects = await _subjectService.GetAll();
        return Ok(subjects.Select(subject => new SubjectDto(subject)));
    }

    [Authorize]
    [MinRoleAuthorize(Role.ScheduleBureau)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateSubject([FromBody] SubjectDto subjectDto)
    {
        await _subjectService.CreateSubject(subjectDto);
        return NoContent();
    }

    [Authorize]
    [MinRoleAuthorize(Role.ScheduleBureau)]
    [HttpPut("edit/{id:guid}")]
    public async Task<IActionResult> EditSubject([FromRoute] Guid id, [FromBody] SubjectDto subjectDto)
    {
        var r = await _subjectService.EditSubject(id, subjectDto);
        return r.Match(NoContent, e => e is RecordNotFoundException ? NotFound() : StatusCode(500));
    }

    [Authorize]
    [MinRoleAuthorize(Role.ScheduleBureau)]
    [HttpPut("delete/{id:guid}")]
    public async Task<IActionResult> DeleteSubject([FromRoute] Guid id)
    {
        var r = await _subjectService.DeleteSubject(id);
        return r.Match(NoContent, e => e is RecordNotFoundException ? NotFound() : StatusCode(500));
    }
}