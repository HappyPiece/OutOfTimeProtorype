using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.General.Implementations;

public class SubjectService : ISubjectService
{
    private readonly OutOfTimeDbContext _outOfTimeDbContext;
    private readonly IMapper _mapper;

    public SubjectService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
    }

    public async Task<List<SubjectDto>> GetAll()
    {
        return await _outOfTimeDbContext.Subjects.Select(subject => _mapper.Map<SubjectDto>(subject)).ToListAsync();
    }

    public async Task CreateSubject(SubjectDto subjectDto)
    {
        var subject = new Subject
        {
            Name = subjectDto.Name
        };

        _outOfTimeDbContext.Subjects.Add(subject);
        await _outOfTimeDbContext.SaveChangesAsync();
    }

    public async Task<Result> EditSubject(Guid id, SubjectDto subjectDto)
    {
        var dbSubject = await _outOfTimeDbContext.Subjects.FindAsync(id);

        if (dbSubject is null)
        {
            return new RecordNotFoundException($"Subject with id '{id.ToString()}' not found");
        }

        var updatedEntity = _mapper.Map(subjectDto, dbSubject);

        _outOfTimeDbContext.Subjects.Update(updatedEntity);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteSubject(Guid id)
    {
        var dbSubject = await _outOfTimeDbContext.Subjects.FindAsync(id);

        if (dbSubject is null)
        {
            return new RecordNotFoundException($"Subject with id '{id.ToString()}' not found");
        }

        _outOfTimeDbContext.Subjects.Remove(dbSubject);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }
}