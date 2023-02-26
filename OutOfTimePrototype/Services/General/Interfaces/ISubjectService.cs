using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Services.General.Interfaces;

public interface ISubjectService
{
    public Subject GetAll();
    public Task Create(SubjectDto subjectDto);
    public Task Update(Guid id, SubjectDto subjectDto);
    public Task Delete(Guid id);
}