using AutoMapper;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Services;

public class SubjectService : ISubjectService
{
    private readonly OutOfTimeDbContext _outOfTimeDbContext;
    private readonly IMapper _mapper;

    public SubjectService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
    }

    public Subject GetAll()
    {
        throw new NotImplementedException();
    }

    public Task Create(SubjectDto subjectDto)
    {
        throw new NotImplementedException();
    }

    public Task Update(Guid id, SubjectDto subjectDto)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}