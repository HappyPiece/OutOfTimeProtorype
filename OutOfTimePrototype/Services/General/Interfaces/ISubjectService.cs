using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.General.Interfaces;

public interface ISubjectService
{
    public Task<List<SubjectDto>> GetAll();
    public Task CreateSubject(SubjectDto subjectDto);
    public Task<Result> EditSubject(Guid id, SubjectDto subjectDto);
    public Task<Result> DeleteSubject(Guid id);
}