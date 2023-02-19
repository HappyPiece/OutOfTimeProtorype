using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.DTO;
using static OutOfTimePrototype.Utilities.ClassUtilities;

namespace OutOfTimePrototype.Services
{
    public interface IClassService
    {
        Task<ClassOperationResult> TryCreateClass(ClassDto ClassDto);
        Task<ClassOperationResult> TryEditClass(Guid id, ClassEditDto classEditDto, bool nullMode);
        Task<ClassOperationResult> TryDeleteClass(Guid id);
        Task<ClassOperationResult> QueryClasses(ClassQueryDto classQueryDto);
    }
}
